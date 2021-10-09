using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web.Configuration;
using System.Web.Http;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models.Air;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Microsoft.Scripting.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Security;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class WalletController : ApiController
    {
        #region -- Constructors --
        public WalletController(IdentityManager identityManager, IWalletService walletService,
            IWalletTransactionService walletTransactionService)
        {
            _identityManager = identityManager;
            _walletTransactionService = walletTransactionService;
            _walletService = walletService;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IWalletService _walletService;
        #endregion

        #region -- Methods --
        public List<dynamic> GetTransactions(string userName, string fromDateStr, string toDateStr)
        {
            var user = _walletService.GetUser(userName, _identityManager); //_identityManager.GetUserFromUserName(userName);

            if (null == user)
                throw new Exception("User (" + userName + ") does not exist in the system.");
            var fromDate = DateTime.ParseExact(fromDateStr, ResponseConstants.DateFormat, CultureInfo.InvariantCulture);
            var toDate = DateTime.ParseExact(toDateStr, ResponseConstants.DateFormat, CultureInfo.InvariantCulture);

            // Create Wallet Transaction
            var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
            var transactions = walletTransactionService.Get(w => w.UserName == user.UserName &&
                 DbFunctions.TruncateTime(w.TransactionDate) >= DbFunctions.TruncateTime(fromDate) &&
                 DbFunctions.TruncateTime(w.TransactionDate) <= DbFunctions.TruncateTime(toDate))
                .Select(w => new
                {
                    w.Id,
                    TransactionId = w.Id.ToString().PadLeft(6, '0'),
                    w.UserName,
                    TransactionDate = w.TransactionDate?.ToString(ResponseConstants.DateFormat),
                    Service = string.IsNullOrEmpty(w.Service) ? string.Empty : w.Service,
                    Operator = string.IsNullOrEmpty(w.Operator) ? string.Empty : w.Operator,
                    Pnr = string.IsNullOrEmpty(w.Pnr) ? string.Empty : w.Pnr,
                    BookingId = string.IsNullOrEmpty(w.BookingId) ? string.Empty : w.BookingId,
                    Source = string.IsNullOrEmpty(w.Source) ? string.Empty : w.Source,
                    Destination = string.IsNullOrEmpty(w.Destination) ? string.Empty : w.Destination,
                    w.Amount,
                    w.Commission,
                    w.OpeningBalance,
                    w.Credit,
                    w.Debit,
                    w.ClosingBalance,
                    w.Status,
                    SourceCity = GetStringFromResponse(w.Service, w.Response, ResponseConstants.SourceCity),
                    DestinationCity = GetStringFromResponse(w.Service, w.Response, ResponseConstants.DestinationCity),
                    DepartureTime = GetStringFromResponse(w.Service, w.Response, ResponseConstants.DepartureTime),
                    ArrivalTime = GetStringFromResponse(w.Service, w.Response, ResponseConstants.ArrivalTime),
                    Duration = GetStringFromResponse(w.Service, w.Response, ResponseConstants.Duration),
                    CarName = GetStringFromResponse(w.Service, w.Response, ResponseConstants.CardName),
                    CardNumber = GetStringFromResponse(w.Service, w.Response, ResponseConstants.CardNo),
                    ExpiryDate = GetStringFromResponse(w.Service, w.Response, ResponseConstants.ExpiryDate),
                    CardPrice = GetStringFromResponse(w.Service, w.Response, ResponseConstants.CardPrice),
                }).ToList().OrderByDescending(w => w.Id);


            return transactions.ToList<dynamic>();
        }
        #endregion
        private string GetStringFromResponse(string service, string response, string cases)
        {
            var requiredString = string.Empty;

            if (service == null)
                return requiredString;

            if (service.ToUpper() == ServiceConstants.Airline.ToUpper())
            {
                var jObject = JsonConvert.DeserializeObject<Response>(JObject.Parse(response.ToString())["Response"]["Response"].ToString()).FlightItinerary.Segments[0];
                switch (cases)
                {
                    case ResponseConstants.SourceCity:
                        requiredString = jObject.Origin.Airport.CityName;
                        break;
                    case ResponseConstants.DestinationCity:
                        requiredString = jObject.Destination.Airport.CityName;
                        break;
                    case ResponseConstants.DepartureTime:
                        requiredString = jObject.StopPointDepartureTime;
                        break;
                    case ResponseConstants.ArrivalTime:
                        requiredString = jObject.StopPointArrivalTime;
                        break;
                    case ResponseConstants.Duration:
                        var mins = jObject.Duration;
                        TimeSpan span = TimeSpan.FromMinutes(mins);
                        requiredString = span.ToString(@"hh\:mm\:ss");
                        break;
                }
            }
            if (service.ToUpper() == ServiceConstants.GiftCard.ToUpper())
            {
                Dictionary<string, JArray> _Data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, JArray>>(((JObject)(JObject.Parse(response))["carddetails"]).ToString());
                foreach (var data in _Data)
                {
                    var cardDetails = JsonConvert.DeserializeObject<CardDetails>(data.Value.ToString().Replace("[", "").Replace("]", ""));
                    switch (cases)
                    {
                        case ResponseConstants.CardName:
                            requiredString = data.Key.ToString();
                            break;
                        case ResponseConstants.CardNo:
                            requiredString = cardDetails.CardNumber;
                            break;
                        case ResponseConstants.CardPrice:
                            requiredString = cardDetails.Card_Price;
                            break;
                        case ResponseConstants.ExpiryDate:
                            requiredString = cardDetails.Expiry_Date;
                            break;
                    }
                }
            }
            return requiredString;
        }


        public HttpResponseMessage CheckWalletBalanceInLimit(RequestModel viewModel)
        {
            var status = "Success";
            var bStatus = true;
            double? walletAmount = 0;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                {
                    var userName = _walletService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    throw new Exception("User (" + userName + ") does not exist in the system.");
                }

                _walletService.CheckWalletBalanceInLimit(user, viewModel.Amount);

                walletAmount = user.Wallet;
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0
            };
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage WalletBalance(RequestModel viewModel)
        {
            var status = "Success";
            var bStatus = true;
            double? walletAmount = 0;
            var iskyc = false; var isPan = false; var isAdhaar = false; var isEmailConfirmed = false;
            var isCardUser = false;
            var cardAmount = string.Empty;
            var fcmToken = string.Empty;

            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                {
                    var userName = _walletService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    throw new Exception("User (" + userName + ") does not exist in the system.");
                }
                iskyc = user.IsKYCSubmit;
                isPan = user.IsPanSubmit;
                isAdhaar = user.IsAdhaarSubmit;
                isEmailConfirmed = user.EmailConfirmed;
                walletAmount = _identityManager.GetWallet(user.Id);
                if (null == walletAmount)
                {
                    walletAmount = 0;
                    _identityManager.UpdateWallet(user.Id, walletAmount);
                }

                var _safexServices = (IJugadUserSafexUserService)Bootstrapper.GetService(typeof(JugadUserSafexUserService));
                var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                if (safexUser != null)
                {
                    isCardUser = safexUser.IsCardUser;
                }

                //if (isCardUser)
                //{
                //    var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                //    var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = viewModel.UserName});
                //    walletAmount = Convert.ToDouble(responseModel.CardAmount);
                //    _identityManager.UpdateWallet(user.Id, walletAmount);
                //}
                cardAmount = WebConfigurationManager.AppSettings["CardAmount"];
                fcmToken = user.FCMToken;

            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0,
                IsKYCSubmit = iskyc,
                IsAdhaarSubmit = isAdhaar,
                IsPanSubmit = isPan,
                IsEmailConfirmed = isEmailConfirmed,
                IsCardUser = isCardUser,
                CardAmount = cardAmount,
                FCMToken = fcmToken
            };
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage FetchWalletBalance(RequestModel viewModel)
        {
            var status = "Success";
            var bStatus = true;
            double? walletAmount = 0;

            try
            {
                var _walletBaseService = (IWalletBaseService)Bootstrapper.GetService(typeof(WalletBaseService));

                // Validate service model
                var user = _walletBaseService.GetUser(viewModel.UserName, _identityManager);
                walletAmount = user.Wallet;
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new walletResponse
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0
            };
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        [HttpPost]
        public HttpResponseMessage GetTransactionDetail(RequestModel viewModel)
        {
            string status;
            double? walletAmount = 0;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = _walletService.GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                {
                    var userName = _walletService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    throw new Exception("User (" + userName + ") does not exist in the system.");
                }

                var transactionId = ConversionHelper.ToInt(viewModel.TransactionId);
                // Create Wallet Transaction
                var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));

                var transaction = walletTransactionService.Get(w => w.UserName == user.UserName &&
                        w.Id == transactionId)
                    .Select(w => new
                    {
                        TransactionId = w.Id.ToString().PadLeft(6, '0'),
                        w.UserName,
                        TransactionDate = w.TransactionDate?.ToString("dd/MM/yyyy"),
                        w.CyberPlatTransId,
                        w.OperatorTransId,
                        w.PaymentTransactionId,
                        w.Pnr,
                        w.BookingId,
                        w.Service,
                        w.Operator,
                        w.Source,
                        w.Destination,
                        w.PaymentMode,
                        w.Amount,
                        w.Commission,
                        w.OpeningBalance,
                        w.Credit,
                        w.Debit,
                        w.ClosingBalance,
                        w.Status,
                        Request = JsonConvert.DeserializeObject(w.Request),
                        Response = JsonConvert.DeserializeObject(w.Response)
                    }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, transaction);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0
            };
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        [HttpPost]
        public HttpResponseMessage WalletTransactions(RequestModel viewModel)
        {
            return GetTransactions(viewModel);
        }

        [HttpPost]
        public HttpResponseMessage GetTransactions(RequestModel viewModel)
        {
            string status;
            double? walletAmount = 0;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var transactions = GetTransactions(viewModel.UserName, viewModel.FromDate, viewModel.ToDate);
                return Request.CreateResponse(HttpStatusCode.OK, transactions);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0
            };
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        [HttpPost]
        public HttpResponseMessage GetMyBookings(RequestModel viewModel)
        {
            string status;
            double? walletAmount = 0;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var transactions = GetTransactions(viewModel.UserName, viewModel.FromDate, viewModel.ToDate);
                var utilityBillsStr = new[]
                {
                    ServiceConstants.MobilePrepaid, ServiceConstants.Dth, ServiceConstants.DataCardPrepaid,
                    ServiceConstants.DataCardPostpaid, ServiceConstants.MobilePostpaid, ServiceConstants.Landline,
                    ServiceConstants.Electricity, ServiceConstants.Gas, ServiceConstants.Insurance,
                    ServiceConstants.Broadband
                };
                var utilityBills = transactions.Where(t => Enumerable.Contains(utilityBillsStr, t.Service));

                var travalesStr = new[]
                {
                    ServiceConstants.Airline, ServiceConstants.Bus,
                    ServiceConstants.Hotel
                };
                var travels = transactions.Where(t => Enumerable.Contains(travalesStr, t.Service));

                var dmtStr = new[] { ServiceConstants.Dmt, ServiceConstants.BalanceTransfer };
                var dmts = transactions.Where(t => Enumerable.Contains(dmtStr, t.Service));

                var giftCardStr = new[] { ServiceConstants.GiftCard };
                var giftCards = transactions.Where(t => Enumerable.Contains(giftCardStr, t.Service));

                var mybookings = new { utilityBills, travels, dmts, giftCards };
                return Request.CreateResponse(HttpStatusCode.OK, mybookings);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = false,
                Result = status,
                WalletBalance = 0
            };
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage Credit(RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.Credit(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage Debit(RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.Debit(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage BalanceTransfer(RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.BalanceTransfer(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        [HttpPost]
        public HttpResponseMessage GetRecentTransaction(RequestModel viewModel)
        {
            var recentTransaction = _walletService.GetRecentTransaction(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, recentTransaction);
        }

        [HttpGet]
        public IEnumerable<SocietyResponseModel> RechargeWallet([FromUri]SocietyRequestModel viewModel)
        {
            var status = true;
            var errorMessage = string.Empty;
            var transaction = new WalletTransaction();
            var transactionId = Guid.NewGuid().ToString();
            var _logService = (ILogService)Bootstrapper.GetService(typeof(LogService));
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                if (viewModel.Amount <= 0)
                    throw new Exception("Amount cannot be 0 or less than 0. Kindly enter a valid amount.");

                LogHandler.LogInfo("search for society start..", LogHandler.LogType.Notify);
                switch (viewModel.SocietyId)
                {
                    case 142:
                        viewModel.CustomerNumber = _walletService.Decrypt(viewModel.CustomerNumber, FieldConstants.SSV);
                        break;
                    case 143:
                        viewModel.CustomerNumber = _walletService.Decrypt(viewModel.CustomerNumber, FieldConstants.SSC);
                        break;
                    case 144:
                        viewModel.CustomerNumber = _walletService.Decrypt(viewModel.CustomerNumber, FieldConstants.HUM);
                        break;
                    case 146:
                        viewModel.CustomerNumber = _walletService.Decrypt(viewModel.CustomerNumber, FieldConstants.LUCC);
                        break;
                }
                LogHandler.LogInfo("Society : " + viewModel.CustomerNumber, LogHandler.LogType.Notify);
                _walletService.ValidateSocietyRequest(viewModel);
                LogHandler.LogInfo("Society validated", LogHandler.LogType.Notify);

                //validate society and check admin wallet
                var societyAdmin = _walletService.GetSocietyAdmin(viewModel.SocietyId, _identityManager);

                if (null == societyAdmin.Wallet || societyAdmin.Wallet <= viewModel.Amount)
                    throw new Exception("Society Wallet has insuffiecient balance");

                LogHandler.LogInfo("Finding reciever..", LogHandler.LogType.Notify);
                //validate user
                var toUser = _walletService.GetUser(viewModel.CustomerNumber, _identityManager); // _identityManager.GetUserFromUserName(viewModel.ToUserName);
                if (null == toUser)
                {
                    var toUserName = _walletService.Decrypt(viewModel.CustomerNumber, FieldConstants.LoginIv);
                    throw new Exception("The account for " + toUserName + "does not exist in 4everPay system !");
                }
                LogHandler.LogInfo("Reciever : "+ toUser.UserName , LogHandler.LogType.Notify);

                LogHandler.LogInfo("Saving society details for customer", LogHandler.LogType.Notify);

                var userSocietyRT = (IUserSocietyRT)Bootstrapper.GetService(typeof(UserSocietyRT));
                var isExist = userSocietyRT.Get(w => w.UserID == toUser.Id).FirstOrDefault();
                if (isExist == null)
                {
                    var societyRT = new SocietyUserRT
                    {
                        SocietyID = viewModel.SocietyId,
                        UserID = toUser.Id
                    };
                    userSocietyRT.Add(societyRT);
                    userSocietyRT.Save();
                }

                toUser.IsSocietyMember = true;
                toUser.SocietyId = viewModel.SocietyId;

                _identityManager.UpdateUser(toUser);

                transaction.PaymentMode = PaymentMode.Wallet;
                transaction.Service = ServiceConstants.BalanceTransfer;
                transaction.DeviceId = viewModel.DeviceId;
                transaction.Operator = ServiceConstants.JugadWallet;
                transaction.TransactionDate = DateTime.Now;
                transaction.Commission = 0;
                transaction.Amount = viewModel.Amount;
                transaction.OperatorTransId = viewModel.RequestTransactionId;
                transaction.PaymentTransactionId = transactionId;
                transaction.Source = societyAdmin.UserName;
                transaction.Destination = toUser.UserName;
                transaction.Status = StatusConstants.Success;
                transaction.CyberPlatTransId = "JW" + Guid.NewGuid().ToString();
                transaction.UserName = toUser.UserName;

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {
                        //var _safexServices = (IJugadUserSafexUserService)Bootstrapper.GetService(typeof(JugadUserSafexUserService));

                        //var isCardUser = _safexServices.Get(x => x.UserName == toUser.UserName).FirstOrDefault();
                        //if (isCardUser != null)
                        //{
                        //    if (isCardUser.IsCardUser)
                        //    {
                        //        var safexRequest = new RequestModel
                        //        {
                        //            Service = transaction.Service,
                        //            Operator = transaction.Operator,
                        //            UserName = transaction.UserName,
                        //            Amount = transaction.Amount,
                        //            CyberPlatTransId = transaction.CyberPlatTransId,
                        //        };

                        //        var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                        //        var safexResult = _prePaidServices.ReflectTransaction(safexRequest);

                        //        if (!safexResult.Status)
                        //        {
                        //            throw new Exception("Reflection at safex failed");
                        //        }
                        //    }
                        //}

                        //Society Admin Debit
                        transaction.OpeningBalance = societyAdmin.Wallet;
                        transaction.Credit = 0;
                        transaction.Debit = viewModel.Amount;
                        transaction.ClosingBalance = societyAdmin.Wallet - transaction.Debit;
                        transaction.UserName = societyAdmin.UserName;
                        var societyBalance = transaction.ClosingBalance;
                        LogHandler.LogInfo("Saving transaction for society", LogHandler.LogType.Notify);
                        _identityManager.UpdateWallet(societyAdmin.Id, transaction.ClosingBalance);
                        _walletTransactionService.Add(transaction);
                        _walletTransactionService.Save();

                        var request = JsonConvert.SerializeObject(transaction);
                        _logService.GenerateLog(ServiceConstants.BalanceTransfer, "Web Control Panel", "Sender", request, "", transaction.UserName);


                        //User Credit
                        transaction.OpeningBalance = toUser.Wallet;
                        transaction.Credit = viewModel.Amount;
                        transaction.Debit = 0;
                        transaction.ClosingBalance = toUser.Wallet + transaction.Credit;
                        transaction.UserName = toUser.UserName;
                        LogHandler.LogInfo("Saving transaction for user", LogHandler.LogType.Notify);
                        _identityManager.UpdateWallet(toUser.Id, transaction.ClosingBalance);
                        _walletTransactionService.Add(transaction);
                        _walletTransactionService.Save();


                        scope.Complete();

                        request = JsonConvert.SerializeObject(transaction);
                        _logService.GenerateLog(ServiceConstants.BalanceTransfer, "Web Control Panel", "Reciever", request, "", transaction.UserName);

                        var message = "Rs. " + viewModel.Amount + " added successfully to your 4everPay wallet (" + toUser.UserName + ").";
                        TboBaseController.SendSms(toUser.UserName, message, string.Empty);
                    }

                    catch (Exception)
                    {
                        scope.Dispose();
                        throw;
                    }
                }
            }
            catch (Exception exception)
            {
                errorMessage = LogHandler.LogError(exception).Message;
                status = false;
            }
            SocietyResponseModel[] postData = new SocietyResponseModel[]
            {
                new SocietyResponseModel
                {
                    Status = status,
                    Message = errorMessage,
                    TimeStamp = viewModel.TimeStamp,
                    RequestTransactionId = viewModel.RequestTransactionId,
                    TokenId = viewModel.TokenId,
                    TransactionId = transactionId
                },
            };

            return postData;
        }

        public HttpResponseMessage WalletCardInterTransaction(RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.WalletCardInterTransaction(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage CardCredit(RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.CardCredit(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage CardDebit(RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.CardDebit(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        public HttpResponseMessage CheckLimit (RequestModel viewModel)
        {
            var walletReturnInfo = _walletService.CheckLimit(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, walletReturnInfo);
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
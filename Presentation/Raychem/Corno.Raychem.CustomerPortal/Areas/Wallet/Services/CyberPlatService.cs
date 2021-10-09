using System;
using System.Transactions;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using CCA.Util;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using System.Globalization;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers;
using System.Linq;
using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CyberPlatService : WalletBaseService, ICyberPlatService
    {
        #region -- Constructors --
        public CyberPlatService(IdentityManager identityManager, IWalletTransactionService walletTransactionService,
            ICyberPlatApiService cyberPlatApiService, ICommissionApiServices commissionService) : base(identityManager)
        {
            _walletTransactionService = walletTransactionService;
            _identityManager = identityManager;
            _cyberPlatApiService = cyberPlatApiService;
            _commissionService = commissionService;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ICyberPlatApiService _cyberPlatApiService;
        private readonly ICommissionApiServices _commissionService;
        #endregion

        #region -- Methods --

        public ResponseModel Validate(RequestModel viewModel)
        {
            var status = "Successful.";
            var bStatus = true;
            var transactionId = string.Empty;
            var addionalInfo = string.Empty;
            var sessionId = string.Empty;
            var conveninceCharges = 0.0;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                // Validate Number
                ValidateNumber(viewModel);

                if (viewModel.Amount <= 0)
                    throw new Exception("This is an invalid Amount. Kindly enter a valid Amount.");

                var user = GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                // Call cyberplat api to recharge here.
                var response = _cyberPlatApiService.ValidationCheck(viewModel);
                transactionId = response.Between("SESSION=", Environment.NewLine);
                addionalInfo = response.Between("ADDINFO=", Environment.NewLine).Replace("\"", "'");

                var comissionOperator = _cyberPlatApiService.GetCommissionOperator(viewModel.Service, viewModel.Operator);
                conveninceCharges = comissionOperator == string.Empty ? 0 : _commissionService.GetConvinienceCharges(viewModel.Service, comissionOperator.Trim(), viewModel.Amount);

                sessionId = Encrypt(DateTime.Now.ToString("ddMMyyhhmm") + transactionId, FieldConstants.SessionIv);
            }
            catch (Exception exception)
            {
                status = exception.Message;
                if (null != exception.InnerException)
                {
                    var response = exception.InnerException.Message;
                    var errorMessage = response.Between("ERRMSG=", Environment.NewLine);
                    if (!string.IsNullOrEmpty(errorMessage))
                        status = errorMessage;
                }
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                TransactionId = transactionId,
                AddionalInfo = addionalInfo,
                SessionId = sessionId,
                ConvinienceCharges = conveninceCharges
            };

            return returnViewModel;
        }

        public ResponseModel Payment(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            double? walletAmount = 0;
            var transactionId = 0;
            var globalUser = new ApplicationUser();

            try
            {
                Logger.LogHandler.LogInfo("User Validation", Logger.LogHandler.LogType.Notify);

                // Validate service model
                var user = ValidateServiceViewModel(viewModel);
                globalUser = user;
                viewModel.UserName = user.UserName;
                Logger.LogHandler.LogInfo("User Validation Successfull", Logger.LogHandler.LogType.Notify);
                Logger.LogHandler.LogInfo("------------------ Transaction Start for" + user.UserName + "-----------------------------", Logger.LogHandler.LogType.Notify);
                
                // Get number for payment
                ValidateNumber(viewModel);

                // Opening Balance
                viewModel.OpeningBalance = user.Wallet;
                Logger.LogHandler.LogInfo("Opening balance " + viewModel.OpeningBalance, Logger.LogHandler.LogType.Notify);

                walletAmount = user.Wallet;

                // Update wallet if payment mode is wallet
                if (viewModel.PaymentMode.ToLower() == PaymentMode.Wallet.ToLower())
                {
                    walletAmount = user.Wallet - viewModel.Amount;
                    viewModel.Credit = 0;
                    viewModel.Debit = viewModel.Amount;
                }

                Logger.LogHandler.LogInfo("Updating User wallet", Logger.LogHandler.LogType.Notify);
                _identityManager.UpdateWallet(user.Id, walletAmount);
                Logger.LogHandler.LogInfo("User wallet Updated" + user.Id, Logger.LogHandler.LogType.Notify);

                //var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

                //if (user.CustId != null && viewModel.PaymentMode.ToLower() == PaymentMode.Wallet.ToLower())
                //{
                //    Logger.LogHandler.LogInfo("Updating User wallet at Safex" + user.Id, Logger.LogHandler.LogType.Notify);
                //    viewModel.Amount = Convert.ToDouble(viewModel.Debit);
                //    var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                //    if (!safexResult.Status)
                //    {
                //        throw new Exception("Reflection at safex failed");
                //    }
                //    Logger.LogHandler.LogInfo("User wallet Updated at Safex" + user.Id, Logger.LogHandler.LogType.Notify);
                //}

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {
                        // Call cyberplat api to recharge here.
                        Logger.LogHandler.LogInfo("CyberPlat payment request start...", Logger.LogHandler.LogType.Notify);
                        var response = _cyberPlatApiService.Payment(viewModel);
                        Logger.LogHandler.LogInfo("CyberPlat payment request completed", Logger.LogHandler.LogType.Notify);

                        viewModel.CyberPlatTransId = response.Between("TRANSID=", Environment.NewLine);
                        viewModel.OperatorTransId = response.Between("AUTHCODE=", Environment.NewLine);

                        Logger.LogHandler.LogInfo("CyberPlat payment request success", Logger.LogHandler.LogType.Notify);

                        viewModel.State = StatusConstants.Success;

                        // Closing Balance
                        viewModel.ClosingBalance = walletAmount;

                        viewModel.UserName = user.UserName;

                        // Create Wallet Transaction
                        transactionId = _walletTransactionService.AddTransaction(viewModel);

                        scope.Complete();

                        Logger.LogHandler.LogInfo("------------------ Transaction Start -----------------------------", Logger.LogHandler.LogType.Notify);
                        var customerMessage = "Dear Customer, INR " + viewModel.Amount + " for " + viewModel.Service + " (" + viewModel.Number + ") on "+ DateTime.Now +" has been done succesfully ";
                        TboBaseController.SendSms(viewModel.UserName, customerMessage, string.Empty);
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
                status = exception.Message;
                if (null != exception.InnerException)
                {
                    var response = exception.InnerException.Message;
                    var errorMessage = response.Between("ERRMSG=", Environment.NewLine);
                    if (!string.IsNullOrEmpty(errorMessage))
                        status = errorMessage;
                }

                //if (!string.IsNullOrEmpty(globalUser.Id))
                //    { RevertAmount(viewModel, globalUser); }

                bStatus = false;

                if (viewModel.PaymentMode.ToLower() != PaymentMode.Wallet.ToLower())
                {
                    var customerMessage = "Transaction for " + viewModel.Service + " (" + viewModel.Number + ") " + "has failed. If Rs." + viewModel.Amount + " is deducted, it will be refunded within 6-7 working days.";
                    TboBaseController.SendSms(viewModel.UserName, customerMessage, string.Empty);
                }
                Logger.LogHandler.LogInfo("------------------ Transaction End -----------------------------", Logger.LogHandler.LogType.Notify);
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0,
                TransactionId = transactionId.ToString().PadLeft(6, '0'),
            };

            return returnViewModel;
        }

        public string GetBalance()
        {
            return _cyberPlatApiService.GetBalance();
        }

        public void MposTransactionCapture(MposResponseModel mposModel)
        {
            var viewModel = new RequestModel();

            var _walletService = (IWalletService)Bootstrapper.GetService(typeof(WalletService));
            mposModel.UserName = _walletService.Encrypt(mposModel.UserName, FieldConstants.LoginIv);

            var user = GetUser(mposModel.UserName, _identityManager);
            try
            {
                viewModel.UserName = mposModel.UserName;

                // Opening Balance
                viewModel.OpeningBalance = user.Wallet;
                viewModel.PaymentMode = PaymentMode.Mpos;
                viewModel.Amount = mposModel.Amount;
                viewModel.Credit = 0;
                viewModel.Debit = mposModel.AmountOriginal + mposModel.AmountAdditional - mposModel.AmountCashBack;
                viewModel.Commission = mposModel.AmountCashBack;
                viewModel.CyberPlatTransId = mposModel.TxnId;
                viewModel.OperatorTransId = mposModel.Mid;
                viewModel.PgTxnId = mposModel.RrNumber;
                viewModel.Operator = "Ezetap";
                viewModel.DeviceId = mposModel.DeviceSerial;
                viewModel.EndUserIp = mposModel.CustomerReceiptUrl;

                // Create Wallet Transaction
                var transactionId = _walletTransactionService.AddTransaction(viewModel);
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message) ;
            }
        }

        [HttpPost]
        public string GetRSA(RequestModel viewModel)
        {
            try
            {
               // string queryUrl = "https://test.ccavenue.com/transaction/getRSAKey"; //TEST url
                string queryUrl = "https://secure.ccavenue.com/transaction/getRSAKey"; // Live Url
                string vParams = "";
                //foreach (string key in Request.Params.AllKeys)
                //{
                vParams = "access_code=" + viewModel.AppKey + "&order_id=" + viewModel.OrderId;
                // }
                // Url Connection
                String message = postPaymentRequestToGateway(queryUrl, vParams);
                return (message);
            }
            catch (Exception exp)
            {
                return ("Exception " + exp);
            }
        }

        private string postPaymentRequestToGateway(String queryUrl, String urlParam)
        {
            String message = "";
            try
            {
                StreamWriter myWriter = null;// it will open a http connection with provided url
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequest objRequest = WebRequest.Create(queryUrl);//send data using objxmlhttp object
                objRequest.Method = "POST";
                //objRequest.ContentLength = TranRequest.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";//to set content type
                myWriter = new System.IO.StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(urlParam);//send data
                myWriter.Close();//closed the myWriter object

                // Getting Response
                System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();//receive the responce from objxmlhttp object 
                using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
                {
                    message = sr.ReadToEnd();
                }
            }
            catch (Exception exception)
            {
                Console.Write("Exception occured while connection." + exception);
            }
            return message;
        }

        public ResponseModel SaveFCMToken(RequestModel viewModel)
        {
            var status = "Save Succesfully";
            var bStatus = true;
            try
            {
                var user = GetUser(viewModel.UserName, _identityManager);

                user.FCMToken = viewModel.FCMToken;
                _identityManager.UpdateUser(user);
            }
            catch(Exception ex)
            {
                bStatus = false;
                status = ex.Message;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status
            };

            return returnViewModel;
        }

        public bool Notification(Notification viewModel, string sendType)
        {
            var status = true;

            var FCMTokens = _identityManager.GetUsersFCMToken();

            switch (sendType)
            {
                case "Merchant":
                    FCMTokens = FCMTokens.FindAll(x => x.UserType == "Merchant");
                    break;
                case "Customer":
                    FCMTokens = FCMTokens.FindAll(x => x.UserType == "Customer");
                    break;
            }
            foreach (var token in FCMTokens)
            {
                WebRequest tRequest = WebRequest.Create(FieldConstants.NotificationURL);
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var objNotification = new
                {
                    to = token.FCMToken,
                    data = new
                    {
                        title = viewModel.Title,
                        body = viewModel.Message,
                        imageUrl = viewModel.ImageUrl
                    }
                };
                string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);

                Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", FieldConstants.Authorization));
                tRequest.Headers.Add(string.Format("Sender: id={0}", FieldConstants.Sender));
                tRequest.ContentLength = byteArray.Length;
                tRequest.ContentType = "application/json";
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String responseFromFirebaseServer = tReader.ReadToEnd();

                                FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(responseFromFirebaseServer);

                                if (response.failure == 1)
                                {
                                    status = false;
                                }

                            }
                        }

                    }
                }
            }
            return status;
        }

        public ResponseModel FetchElectricityBillDetails(RequestModel viewModel)
        {
            var status = string.Empty;
            var bStatus = true;
            var addionalInfo = string.Empty;
            var electricityDetails = new ElectricityDetails();
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                // Validate Number
                ValidateNumber(viewModel);

                if (viewModel.Amount <= 0)
                    throw new Exception("This is an invalid Amount. Kindly enter a valid Amount.");

                var user = GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                // Call cyberplat api to recharge here.
                var response = _cyberPlatApiService.ValidationCheck(viewModel);
                var Error = response.Between("ERROR=", Environment.NewLine);
                status = response.Between("ERRMSG=", Environment.NewLine);
                if(Error == "0")
                {
                    addionalInfo = response.Between("ADDINFO=", Environment.NewLine).Replace("\"", "'");
                    if (!String.IsNullOrEmpty(addionalInfo))
                    {
                        var details = addionalInfo.Split('>');
                        electricityDetails.BillNo = details[0].Replace("<", "");
                        electricityDetails.BillDate = details[1].Contains("NA")?"NA":StringToDate(details[1].Replace("<", ""));
                        electricityDetails.DueDate = details[2].Contains("NA") ? "NA" : StringToDate(details[2].Replace("<", ""));
                        electricityDetails.Amount = details[3].Contains("NA") ? 0.0 : Convert.ToDouble(details[3].Replace("<",""));
                    }
                }
            }
            catch (Exception exception)
            {
                var response = exception.InnerException.Message;
                addionalInfo = response.Between("ADDINFO=", Environment.NewLine).Replace("\"", "'");
                status = response.Between("ERRMSG=", Environment.NewLine);
                if (response.Between("ERROR=", Environment.NewLine) == "7" && !String.IsNullOrEmpty(addionalInfo))
                {
                    var details = addionalInfo.Split('>');
                    electricityDetails.BillNo = details[0].Replace("<", "");
                    electricityDetails.BillDate = details[1].Contains("NA") ? "NA" : StringToDate(details[1].Replace("<", ""));
                    electricityDetails.DueDate = details[2].Contains("NA") ? "NA" : StringToDate(details[2].Replace("<", ""));
                    electricityDetails.Amount = details[3].Contains("NA") ? 0.0 : Convert.ToDouble(details[3].Replace("<", ""));
                    status = "";
                    bStatus = true;
                }
                else
                {
                    status = string.IsNullOrEmpty(status)? "Either no bill pending/ due date has passed/ invalid biller details" : status;
                    bStatus = false;
                }
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                ElectricityDetails = electricityDetails
            };

            return returnViewModel;
        }

        public ResponseModel FetchOperator(RequestModel viewModel)
        {
            var status = "Success";
            var bStatus = true;
            var operatorResult = string.Empty;
            var circleResult = string.Empty;

            try
            {
                var _prePaidServices = (IOperatorSeriesRTService)Bootstrapper.GetService(typeof(OperatorSeriesRTService));
                var info = _prePaidServices.Get(x => x.Series == viewModel.Number).FirstOrDefault();
                operatorResult = info.Operator;
                circleResult = info.Circle;
            }

            catch(Exception ex)
            {
                status = "Series doesnt exist";
                bStatus = false;
            }
            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                AddionalInfo = operatorResult,
                DataHtml = circleResult
            };

            return returnViewModel;
        }

        public ResponseModel FetchPrepaidPlans(RequestModel viewModel)
        {
            var status = string.Empty;
            var bStatus = true;
            var info = new List<PrepaidPlans>();
            dynamic prepaidPlans = null;

            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var _prepaidPlansServices = (IPrepaidPlansService)Bootstrapper.GetService(typeof(PrepaidPlansService));
                info = _prepaidPlansServices.Get(x => x.Operator == viewModel.Operator && x.Circle == viewModel.State).OrderBy(z => z.Amount).ToList();

                var GData = info.Where(t => t.Category == RechargePlansConstant.GData).OrderBy(p => p.Amount);
                var FTT = info.Where(t => t.Category == RechargePlansConstant.FTT).OrderBy(p => p.Amount);
                var IR = info.Where(t => t.Category == RechargePlansConstant.IR).OrderBy(p => p.Amount);
                var NR = info.Where(t => t.Category == RechargePlansConstant.NR).OrderBy(p => p.Amount);
                var SMR = info.Where(t => t.Category == RechargePlansConstant.SMR).OrderBy(p => p.Amount);
                var NC = info.Where(t => t.Category == RechargePlansConstant.NC).OrderBy(p => p.Amount);
                var SPR = info.Where(t => t.Category == RechargePlansConstant.SPR).OrderBy(p => p.Amount);
                var TU = info.Where(t => t.Category == RechargePlansConstant.TU).OrderBy(p => p.Amount);


                prepaidPlans = new { GData, FTT, IR, NR, SMR, NC, SPR, TU };


            }
            catch (Exception exception)
            {
                bStatus = false;
                status = exception.Message;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                PrepaidPlans = prepaidPlans
            };
            return walletReturnInfo;
        }

        private string StringToDate(string date)
        {
            var year = date.Substring(1, 4);
            var month = date.Substring(5, 2);
            var day = date.Substring(7, 2);
            var fullDate = month + "/" + day + "/" + year;
            var dt = new DateTime();
            DateTime.TryParseExact(fullDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);

            return dt.ToLongDateString();
        }

        private void RevertAmount(RequestModel viewModel, ApplicationUser user)
        {
            double? walletAmount = 0;
            var transactionId = 0;
            Logger.LogHandler.LogInfo("Revert Money", Logger.LogHandler.LogType.Notify);

            // Update wallet if payment mode is wallet
            walletAmount = viewModel.OpeningBalance;
            _identityManager.UpdateWallet(user.Id, walletAmount);

            //var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

            //if (user.CustId != null && viewModel.PaymentMode.ToLower() == PaymentMode.Wallet.ToLower())
            //{
            //    Logger.LogHandler.LogInfo("Updating User wallet at Safex" + user.Id, Logger.LogHandler.LogType.Notify);
            //    viewModel.Amount = Convert.ToDouble(viewModel.Debit);
            //    viewModel.Service = ServiceConstants.Credit;

            //    var safexResult = _prePaidServices.ReflectTransaction(viewModel);
            //    if (!safexResult.Status)
            //    {
            //        throw new Exception("Reflection at safex failed");
            //    }
            //    Logger.LogHandler.LogInfo("User wallet Updated at Safex" + user.Id, Logger.LogHandler.LogType.Notify);
            //}

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
            {
                try
                {
                    viewModel.State = StatusConstants.Failure;

                    // Closing Balance
                    viewModel.ClosingBalance = walletAmount;

                    // Create Wallet Transaction
                    transactionId = _walletTransactionService.AddTransaction(viewModel);
                    scope.Complete();
                }
                catch (Exception)
                {
                    scope.Dispose();
                    throw;
                }

                Logger.LogHandler.LogInfo("Reverted Successfully", Logger.LogHandler.LogType.Notify);
            }
        }
            #endregion
        }
}
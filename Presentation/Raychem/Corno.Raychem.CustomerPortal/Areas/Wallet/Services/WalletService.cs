using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web.Configuration;
using System.Web.Http;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Newtonsoft.Json;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class WalletService : WalletBaseService, IWalletService
    {
        #region -- Constructors --
        public WalletService(IdentityManager identityManager, IWalletTransactionService walletTransactionService,
            ILogService logService, ISocietyService societyService,IWalletBaseService walletBaseService)
            : base(identityManager)
        {
            _walletTransactionService = walletTransactionService;
            _identityManager = identityManager;
            _logService = logService;
            _societyService = societyService;
            _walletBaseService = walletBaseService;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ILogService _logService;
        private readonly ISocietyService _societyService;
        private readonly IWalletBaseService _walletBaseService;
        #endregion

        #region -- Methods --
        public void CheckWalletBalanceInLimit(ApplicationUser user, double amount)
        {
            if (!user.IsSocietyMember)
            {
                // var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
                //var transactedAmount = walletTransactionService.Get(w => w.UserName == user.UserName &&
                //                                                         w.TransactionDate.Value.Month == DateTime.Now.Month)
                //    .Sum(w => w.Amount);

                //var creditAmount = walletTransactionService.Get(w => w.UserName == user.UserName 
                //                                             && w.TransactionDate.Value.Year == DateTime.Now.Year
                //                                             && w.TransactionDate.Value.Month == DateTime.Now.Month
                //                                             && w.TransactionDate.Value.Day == DateTime.Now.Day
                //                                             && w.Service == ServiceConstants.Credit).Sum(w => w.Amount);

                // double walletLimit = 0;
                double creditLimit = user.IsKYCSubmit ? 10000 : 1000;
                //switch (user.UserType)
                //{
                //    case UserTypeConstants.Customer:
                //        //walletLimit = user.IsKYCSubmit ? 10000 : 1000;
                //        creditLimit = user.IsKYCSubmit ? 10000 : 1000;
                //        break;

                //    case UserTypeConstants.Merchant:
                //        //walletLimit = user.IsKYCSubmit ? 10000 : 1000;
                //        creditLimit = user.IsKYCSubmit ? 10000 : 1000;
                //        break;
                //}

                //var consumedAmount = (double)user.Wallet + transactedAmount;
                //if (consumedAmount + amount > walletLimit)
                //{
                //    throw new Exception("You have exhausted the wallet limit of ₹" + walletLimit + " remaining limit is " + Math.Max(0, (int)(walletLimit - consumedAmount)) + ", to enhace the limit submit KYC or contact 022-28272727 for futher assistance");
                //}

                //var consumedAmount = (double)user.Wallet;
                //if (consumedAmount + amount > creditLimit)
                //{
                //    throw new Exception("You have exahusted the credit limit of ₹" + creditLimit + ", to enhace the limit submit KYC or contact 022-28272727 for futher assistance");
                //}
            }
        }

        public ResponseModel Credit(RequestModel viewModel)
        {
            var status = "Credit Successful";
            var bStatus = true;
            double? walletAmount = 0;
            var transactionId = 0;
            var isCardUser = false;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                if (viewModel.PaymentMode != PaymentMode.Appnit && viewModel.PaymentMode != PaymentMode.Mpos &&
                    viewModel.PaymentMode != PaymentMode.Web && viewModel.PaymentMode != PaymentMode.KotakPG)
                    throw new Exception("Invalid payment mode for credit.");

                Logger.LogHandler.LogInfo("User validation", Logger.LogHandler.LogType.Notify);

                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                Logger.LogHandler.LogInfo("User validation Succesfull" + user.UserName, Logger.LogHandler.LogType.Notify);

                if (viewModel.PaymentMode != PaymentMode.Web)
                    CheckWalletBalanceInLimit(user, viewModel.Amount);  // Check Wallet Limit

                Logger.LogHandler.LogInfo("Payment Mode" + viewModel.PaymentMode, Logger.LogHandler.LogType.Notify);

                //var _safexServices = (IJugadUserSafexUserService)Bootstrapper.GetService(typeof(JugadUserSafexUserService));
                //var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                //if (safexUser != null)
                //{
                //    isCardUser = safexUser.IsCardUser;
                //}

                //if (isCardUser)
                //{
                //    var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                //    var userName = _prePaidServices.Encrypt(viewModel.UserName);
                //    var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = userName});
                //    user.Wallet = Convert.ToDouble(responseModel.CardAmount);
                //    _identityManager.UpdateWallet(user.Id, user.Wallet);
                //}

                using (var scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted,
                        Timeout = TransactionManager.MaximumTimeout
                    }))
                {
                    try
                    {
                        // Opening Balance
                        viewModel.OpeningBalance = Math.Round((double)user.Wallet, 4);
                        Logger.LogHandler.LogInfo("Opening Balance" + viewModel.OpeningBalance, Logger.LogHandler.LogType.Notify);

                        walletAmount = Math.Round((double)user.Wallet, 4);
                        if (null == walletAmount)
                            walletAmount = viewModel.Amount;
                        else
                            walletAmount = Math.Round((double)(viewModel.Amount + walletAmount), 4);

                        viewModel.Credit = viewModel.Amount;
                        Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                        viewModel.Debit = 0;


                        // Closing Balance
                        viewModel.ClosingBalance = Math.Round((double)walletAmount, 4);
                        viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId)?"JW" + Guid.NewGuid().ToString():viewModel.CyberPlatTransId;

                        //if (viewModel.Service != ServiceConstants.CardOperations && user.CustId != null)
                        //{
                        //    viewModel.Service = "Credit";

                        //    var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                        //    var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                        //    if (!safexResult.Status)
                        //    {
                        //        throw new Exception("Reflection at safex failed");
                        //    }

                        //}
                        viewModel.UserName = user.UserName;
                        transactionId = _walletTransactionService.AddTransaction(viewModel);
                        Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                        _identityManager.UpdateWallet(user.Id, walletAmount);
                        Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                        scope.Complete();
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
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0,
                TransactionId = transactionId.ToString().PadLeft(6, '0')
            };

            // Generate Log
            var request = JsonConvert.SerializeObject(viewModel);
            var response = JsonConvert.SerializeObject(walletReturnInfo);
            _logService.GenerateLog(ServiceConstants.Credit, "Web Control Panel", "Credit", request, response, viewModel.UserName);

            return walletReturnInfo;
        }

        public ResponseModel Debit(RequestModel viewModel)
        {
            var status = "Debit Successful";
            var bStatus = true;
            double? walletAmount = 0;
            var transactionId = 0;
            var isCardUser = false;

            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");
                if (viewModel.PaymentMode != PaymentMode.Appnit && viewModel.PaymentMode != PaymentMode.Mpos &&
                    viewModel.PaymentMode != PaymentMode.Wallet && viewModel.PaymentMode != PaymentMode.Web && viewModel.PaymentMode != PaymentMode.KotakPG)
                    throw new Exception("PaymentMode' is not a valid Payment mode!");

                Logger.LogHandler.LogInfo("User validation", Logger.LogHandler.LogType.Notify);

                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                Logger.LogHandler.LogInfo("User Validation Successfull", Logger.LogHandler.LogType.Notify);

                if (viewModel.Service == ServiceConstants.Card)
                {
                    viewModel.Operator = ServiceConstants.JugadAdmin;
                    var comission = new Commission
                    {
                        ServiceId = 189,
                        ConvinienceFee = 211.86,
                        Gst = 18,
                        GstAmount = 38.14,
                        Code = viewModel.BookingId
                    };

                    var _commissionService = (ICommissionService)Bootstrapper.GetService(typeof(CommissionService));
                    _commissionService.AddCommission(comission);
                }

                //var _safexServices = (IJugadUserSafexUserService)Bootstrapper.GetService(typeof(JugadUserSafexUserService));
                //var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                //if (safexUser != null)
                //{
                //    isCardUser = safexUser.IsCardUser;
                //}

                //if (isCardUser)
                //{
                //    var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                //    var userName = _prePaidServices.Encrypt(viewModel.UserName);
                //    var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = userName });
                //    user.Wallet = Convert.ToDouble(responseModel.CardAmount);
                //    _identityManager.UpdateWallet(user.Id, user.Wallet);
                //}
               

                //Check Wallet Limit
                CheckWalletBalanceInLimit(user, viewModel.Amount);

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {
                        // Opening Balance
                        viewModel.OpeningBalance = user.Wallet;
                        Logger.LogHandler.LogInfo("Opening Balance" + viewModel.OpeningBalance, Logger.LogHandler.LogType.Notify);

                        walletAmount = user.Wallet;

                        if (viewModel.PaymentMode == PaymentMode.Wallet || viewModel.PaymentMode == PaymentMode.Web)
                        {

                            if (null == walletAmount || walletAmount <= 0 || walletAmount < viewModel.Amount)
                                throw new Exception("Your account has Insufficient balance.");

                            walletAmount -= viewModel.Amount;

                            viewModel.Credit = 0;
                            viewModel.Debit = viewModel.Amount;
                            Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);

                            viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId)? "JW" + Guid.NewGuid().ToString(): viewModel.CyberPlatTransId;
                        }
                        // Closing Balance
                        viewModel.ClosingBalance = walletAmount;

                        Logger.LogHandler.LogInfo("Update wallet for" + user.Id, Logger.LogHandler.LogType.Notify);
                        _identityManager.UpdateWallet(user.Id, walletAmount);
                        Logger.LogHandler.LogInfo("update wallet successfull", Logger.LogHandler.LogType.Notify);

                      //  Logger.LogHandler.LogInfo("Reflection at safex", Logger.LogHandler.LogType.Notify);

                        //if (viewModel.Service != ServiceConstants.CardOperations && user.CustId != null &&
                        //    (viewModel.PaymentMode.ToLower() == PaymentMode.Wallet.ToLower() || viewModel.PaymentMode.ToLower() == PaymentMode.Web.ToLower()))
                        //{
                        //    var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                        //    var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                        //    if (!safexResult.Status)
                        //    {
                        //        throw new Exception(safexResult.Result);
                        //    }
                        //}

                        if (viewModel.Service != ServiceConstants.GiftCard)
                        {
                            viewModel.Service = "Debit";
                            viewModel.UserName = user.UserName;
                            transactionId = _walletTransactionService.AddTransaction(viewModel);
                        }

                        scope.Complete();
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
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0,
                TransactionId = transactionId.ToString().PadLeft(6, '0')
            };

            // Generate Log
            var request = JsonConvert.SerializeObject(viewModel);
            var response = JsonConvert.SerializeObject(walletReturnInfo);
            _logService.GenerateLog(ServiceConstants.Debit, "Web Control Panel", "Debit", request, response, viewModel.UserName);

            return walletReturnInfo;
        }
        public ApplicationUser GetSocietyAdmin(int societyId, IdentityManager identityManager)
        {
            var societyService = (ISocietyService)Bootstrapper.GetService(typeof(SocietyService));
            var society = societyService.Get(a => a.Id == societyId).FirstOrDefault();

            if (society == null)
                throw new Exception("Society doesn't exist with Jugad system");

            return identityManager.GetUserFromUserName(society.AdminUsername);
        }

        public void ValidateSocietyRequest(SocietyRequestModel requestModel)
        {
            // Validate User Agent
            _walletBaseService.ValidateUserAgent(requestModel.TokenId);

            var user = GetUser(requestModel.CustomerNumber, _identityManager);
            if (null == user)
                throw new Exception(requestModel.CustomerNumber + "'s Account does not exist!");

            if (user.LockoutEnabled)
                throw new Exception(requestModel.CustomerName + "'s account is locked");
            //var request = new RequestModel
            //{
            //    DeviceId = requestModel.DeviceId
            //};

            //// Validate Device ID
            //_walletBaseService.ValidateDeviceId(request, user);
        }

        public ResponseModel CheckLimit(RequestModel viewModel)
        {
            var errorMessage = string.Empty;
            var amount= 0.0;
            var limit = 0.0;
            var consumedAmount = 0.0;
            try
            {
                var user = GetUser(viewModel.UserName, _identityManager);

                var request = new RequestModel
                {
                    DeviceId = viewModel.DeviceId
                };


                // Validate Device ID
                if (viewModel.PaymentMode != PaymentMode.Web)
                    _walletBaseService.ValidateDeviceId(request, user);

                if (!user.EmailConfirmed || !user.PhoneNumberConfirmed)
                {
                    return new ResponseModel
                    {
                        Status = false,
                        Result = "Please confirm email-id and phone number before proceeding"
                    };
                }

                if (user.LockoutEnabled)
                {
                    return new ResponseModel
                    {
                        Status = false,
                        Result = "User is locked"
                    };
                }

                if (user.IsKYCSubmit)
                {
                    return new ResponseModel
                    {
                        Status = true,
                        Result = "User is a full KYC user"
                    };
                }

                //var _safexServices = (IJugadUserSafexUserService)Bootstrapper.GetService(typeof(JugadUserSafexUserService));
                //var safexUser = _safexServices.Get(x => x.UserName == user.UserName).FirstOrDefault();
                //if (safexUser == null)
                //{
                //    return new ResponseModel
                //    {
                //        Status = true,
                //        Result = string.Empty
                //    };
                //}
                //if (!user.IsAdhaarSubmit)
                //{
                //    return new ResponseModel
                //    {
                //        Status = false,
                //        Result = "Please submit e-KYC before proceeding"
                //    };
                //}

                var walletTransactionService = (IWalletTransactionService)Bootstrapper.GetService(typeof(WalletTransactionService));
                //if (user.IsSocietyMember)
                //{
                //    return new ResponseModel
                //    {
                //        Status = true,
                //        Result = string.Empty
                //    };
                //}

                if (viewModel.Service == ServiceConstants.Credit || viewModel.Service == "Wallet")
                {
                    amount = walletTransactionService.Get(w => w.UserName == user.UserName
                                                             && w.TransactionDate.Value.Year == DateTime.Now.Year
                                                             && w.TransactionDate.Value.Month == DateTime.Now.Month
                                                        && (w.Service == ServiceConstants.Credit || w.Service == "Wallet") && w.Status != StatusConstants.Failure).Sum(w => w.Amount);

                    limit = user.IsKYCSubmit ? 100000 : 10000;

                    consumedAmount = amount + viewModel.Amount;  //+ amount
                    if (consumedAmount > limit)
                    {
                        return new ResponseModel
                        {
                            Status = false,
                            Result = "You have exahusted the monthly credit limit of ₹" + limit + "."
                        };
                    }

                    //amount = walletTransactionService.Get(w => w.UserName == user.UserName
                    //                     && w.TransactionDate.Value.Year == DateTime.Now.Year
                    //                && (w.Service == ServiceConstants.Credit || w.Service == "Wallet") && w.Status != StatusConstants.Failure).Sum(w => w.Amount);

                    //limit = user.IsKYCSubmit ? 200000 : 100000;

                    //consumedAmount = amount + viewModel.Amount;  //+ amount
                    //if (consumedAmount > limit)
                    //{
                    //    return new ResponseModel
                    //    {
                    //        Status = false,
                    //        Result = "You have exahusted the yearly credit limit of ₹" + limit + "."
                    //    };
                    //}
                }
                else if (viewModel.Service == ServiceConstants.Dmt && viewModel.Type == "3")
                {
                    amount = walletTransactionService.Get(w => w.UserName == user.UserName
                                                             && w.TransactionDate.Value.Year == DateTime.Now.Year
                                                             && w.TransactionDate.Value.Month == DateTime.Now.Month && w.TransactionDate.Value.Day == DateTime.Now.Day
                                                        && w.Service == ServiceConstants.Dmt && w.Status != StatusConstants.Failure).Sum(w => w.Amount);

                    limit = user.IsSocietyMember ? 25000 : 5000;

                    consumedAmount = amount + viewModel.Amount;  //+ amount
                    if (consumedAmount > limit)
                    {
                        return new ResponseModel
                        {
                            Status = false,
                            Result = "You have exahusted the daily DMT limit of ₹" + limit + ".Try again after 24hrs."
                        };
                    }
                }
                else
                {
                    amount = walletTransactionService.Get(w => w.UserName == user.UserName
                                                                                && w.TransactionDate.Value.Year == DateTime.Now.Year
                                                                                && w.TransactionDate.Value.Month == DateTime.Now.Month
                                                                           && (w.PaymentMode == PaymentMode.Wallet) && w.Status != StatusConstants.Failure).Sum(w => w.Amount);

                    limit = user.IsKYCSubmit ? 100000 : 10000;

                    consumedAmount = amount + viewModel.Amount;  //+ amount
                    if (consumedAmount > limit)
                    {
                        return new ResponseModel
                        {
                            Status = false,
                            Result = "You have exahusted the monthly KYC limit for wallet transactions ₹" + limit + "."
                        };
                    }

                    //amount = walletTransactionService.Get(w => w.UserName == user.UserName
                    //                                        && w.TransactionDate.Value.Year == DateTime.Now.Year
                    //                                   && (w.PaymentMode == PaymentMode.Wallet) && w.Status != StatusConstants.Failure).Sum(w => w.Amount);

                    //limit = user.IsKYCSubmit ? 300000 : 100000;

                    //consumedAmount = amount + viewModel.Amount;  //+ amount
                    //if (consumedAmount > limit)
                    //{
                    //    return new ResponseModel
                    //    {
                    //        Status = false,
                    //        Result = "You have exahusted the yearly KYC limit for wallet transactions ₹" + limit + "."
                    //    };
                    //}
                }

                return new ResponseModel
                {
                    Status = true,
                    Result = string.Empty
                };

            }
            catch (Exception exception)
            {
                return new ResponseModel
                {
                    Status = false,
                    Result = exception.Message
                };
            }
        }

        public ResponseModel BalanceTransfer(RequestModel viewModel)
        {
            var status = "Balance Transfer Successful";
            var bStatus = true;
            double? fromWalletAmount = 0;
            var transactionId = 0;
            var request = string.Empty;
            var isFromExist = false;
            var isToExist = false;

            var _logService = (ILogService)Bootstrapper.GetService(typeof(LogService));

            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                if (viewModel.Amount <= 0)
                    throw new Exception("Amount cannot be 0 or less than 0. Kindly enter a valid amount.");

                viewModel.PaymentMode = PaymentMode.Wallet;
                viewModel.CyberPlatTransId = "JW" + Guid.NewGuid().ToString();

                var fromUser = ValidateServiceViewModel(viewModel);

                //var fromUser = _walletService.GetUser(viewModel.UserName, _identityManager);
                if (null == fromUser)
                    throw new Exception("Account does not exist for " + viewModel.UserName + " in the system.");

                fromWalletAmount = fromUser.Wallet;
                if (null == fromWalletAmount || fromWalletAmount <= viewModel.Amount)
                    throw new Exception("Your account has Insufficient balance.");

                var toUser = GetUser(viewModel.ToUserName, _identityManager); // _identityManager.GetUserFromUserName(viewModel.ToUserName);
                if (null == toUser)
                {
                    var toUserName = Decrypt(viewModel.ToUserName, FieldConstants.LoginIv);
                    throw new Exception("The account for " + toUserName + "does not exist !");
                }
                viewModel.Number = toUser.UserName;

                CheckWalletBalanceInLimit(fromUser, viewModel.Amount);

                if (fromUser.UserName == toUser.UserName)
                    throw new Exception("The Sender and Reciever cannot be same");

                var _safexServices = (IJugadUserSafexUserService)Bootstrapper.GetService(typeof(JugadUserSafexUserService));

                var isFrom = _safexServices.Get(x => x.UserName == viewModel.UserName && x.Status == StatusConstants.Active).FirstOrDefault();
                var isTo = _safexServices.Get(x => x.UserName == viewModel.Number && x.Status == StatusConstants.Active).FirstOrDefault();

                var _prePaidServices = (IPrePaidCardService)Bootstrapper.GetService(typeof(PrePaidCardService));

                if (isFrom != null)
                    isFromExist = isFrom.IsCardUser;

                if (isTo != null)
                    isToExist = isTo.IsCardUser;

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {

                        if (isFromExist && !isToExist)
                        {
                            viewModel.Service = ServiceConstants.Debit;
                            var safexResult = _prePaidServices.ReflectTransaction(viewModel);
                            if (!safexResult.Status)
                            {
                                throw new Exception(safexResult.Result);
                            }

                        }

                        else
                        {
                            viewModel.Service = ServiceConstants.BalanceTransfer;

                            // Opening Balance
                            viewModel.OpeningBalance = fromUser.Wallet;
                            fromWalletAmount -= viewModel.Amount;
                            viewModel.Credit = 0;
                            viewModel.Debit = viewModel.Amount;
                            _identityManager.UpdateWallet(fromUser.Id, fromWalletAmount);
                            // Closing Balance
                            viewModel.ClosingBalance = fromWalletAmount;
                            viewModel.State = StatusConstants.Success;
                            //viewModel.UserName = _walletService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                            transactionId = _walletTransactionService.AddTransaction(viewModel);

                            request = JsonConvert.SerializeObject(viewModel);
                            _logService.GenerateLog(ServiceConstants.BalanceTransfer, "Web Control Panel", "Sender", request, "", viewModel.UserName);
                        }

                        // Opening Balance
                        viewModel.OpeningBalance = toUser.Wallet;
                        viewModel.ToUserName = fromUser.UserName;
                        var toWalletAmount = toUser.Wallet;
                        if (null == toWalletAmount)
                            toWalletAmount = viewModel.Amount;
                        else
                            toWalletAmount += viewModel.Amount;
                        viewModel.Credit = viewModel.Amount;
                        viewModel.Debit = 0;
                        // Closing Balance
                        viewModel.ClosingBalance = toWalletAmount;
                        _identityManager.UpdateWallet(toUser.Id, toWalletAmount);

                        if (!(!isFromExist && !isToExist))
                        {
                            if (!(isFromExist && !isToExist))
                            {
                                viewModel.UserName = fromUser.UserName;
                                var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                                if (!safexResult.Status)
                                {
                                    throw new Exception(safexResult.Result);
                                }

                            }
                        }

                        viewModel.UserName = toUser.UserName;
                        transactionId = _walletTransactionService.AddTransaction(viewModel);

                        request = JsonConvert.SerializeObject(viewModel);
                        _logService.GenerateLog(ServiceConstants.BalanceTransfer, "Web Control Panel", "Reciever", request, "", viewModel.UserName);

                        var customerMessage = "Rs. " + viewModel.Amount + " added successfully to your Jugad wallet.";
                        TboBaseController.SendSms(toUser.UserName, customerMessage, string.Empty);
                        customerMessage = "Rs. " + viewModel.Amount + " sent successfully to Jugad wallet of " + toUser.FirstName + " " + toUser.LastName + " (" + toUser.UserName + ").";
                        TboBaseController.SendSms(fromUser.UserName, customerMessage, string.Empty);

                        scope.Complete();
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
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            return new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = fromWalletAmount ?? 0,
                TransactionId = transactionId.ToString().PadLeft(6, '0')
            };
        }

        public ResponseModel WalletCardInterTransaction(RequestModel viewModel)
        {
            var status = "Transaction Failed, please try after sometime";
            var bStatus = true;
            var transactionId = 0;
            double? walletAmount = 0.0;
            double? cardAmount = 0.0;

            if (null == viewModel)
                throw new Exception("Bad Request");

            Logger.LogHandler.LogInfo("User validation", Logger.LogHandler.LogType.Notify);

            // Validate service model
            var user = ValidateServiceViewModel(viewModel);

            Logger.LogHandler.LogInfo("User validation Succesfull" + user.UserName, Logger.LogHandler.LogType.Notify);

            var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

                switch (viewModel.RoutingType)
                {
                    case "LoadWallet":

                        try
                        {
                            var userName = _prePaidServices.Encrypt(viewModel.UserName);
                            var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = userName });
                            var cardOpeningBalance = Convert.ToDouble(responseModel.CardAmount);

                            if(cardOpeningBalance < viewModel.Amount)
                            {
                                throw new Exception("Insufficient balance in card");
                            }

                            var safexResult = _prePaidServices.ReflectTransaction(viewModel);
                            if (!safexResult.Status)
                            {
                                throw new Exception("Reflection at safex failed");
                            }

                            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                            {
                                try
                                {
                                    //---------- DEBIT CARD ENTRY

                                    // Opening Balance
                                    viewModel.OpeningBalance = cardOpeningBalance;
                                    Logger.LogHandler.LogInfo("Opening Balance " + cardOpeningBalance, Logger.LogHandler.LogType.Notify);

                                    cardAmount = viewModel.OpeningBalance - viewModel.Amount;

                                    viewModel.Debit = viewModel.Amount;
                                    Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                                    viewModel.Credit = 0;

                                    // Closing Balance
                                    viewModel.ClosingBalance = cardAmount;
                                    viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId;

                                    viewModel.UserName = user.UserName;
                                    viewModel.Number = user.CustId;
                                    transactionId = _walletTransactionService.AddTransaction(viewModel);
                                    Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                                    _identityManager.UpdateCard(user.Id, cardAmount);
                                    Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                                //---------- CREDIT WALLET ENTRY

                                    walletAmount = user.Wallet;

                                    // Opening Balance
                                    viewModel.Service = ServiceConstants.Credit;
                                    viewModel.OpeningBalance = user.Wallet;
                                    Logger.LogHandler.LogInfo("Opening Balance" + viewModel.OpeningBalance, Logger.LogHandler.LogType.Notify);

                                    walletAmount = Math.Round((double)(viewModel.Amount + walletAmount), 2);

                                    viewModel.Credit = viewModel.Amount;
                                    Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                                    viewModel.Debit = 0;

                                    // Closing Balance
                                    viewModel.ClosingBalance = walletAmount;
                                    viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId;

                                    viewModel.UserName = user.UserName;
                                    transactionId = _walletTransactionService.AddTransaction(viewModel);
                                    Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                                    _identityManager.UpdateWallet(user.Id, walletAmount);
                                    Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                                    scope.Complete();
                                }
                                catch (Exception)
                                {
                                    scope.Dispose();
                                    throw;
                                }
                            }
                            status = "Amount of Rs." + viewModel.Amount + "/- credited to user wallet (" + user.UserName + ") successfully.";
                        }
                        catch (Exception exception)
                        {
                            status = LogHandler.LogError(exception).Message;
                            bStatus = false;
                        }

                        break;

                    case "LoadCard":

                        try
                        {
                            viewModel.Service = ServiceConstants.Credit;
                            if (user.Wallet < viewModel.Amount)
                            {
                                throw new Exception("Insufficient balance in card");
                            }


                        var userName = _prePaidServices.Encrypt(viewModel.UserName);
                        var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = userName });
                        cardAmount = Convert.ToDouble(responseModel.CardAmount);

                        var safexResult = _prePaidServices.ReflectTransaction(viewModel);
                            if (!safexResult.Status)
                            {
                                throw new Exception("Reflection at safex failed");
                            }

                            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                            {
                                try
                                {
                                    //---------- CREDIT CARD ENTRY

                                    // Opening Balance
                                    viewModel.OpeningBalance = cardAmount;
                                    Logger.LogHandler.LogInfo("Opening Balance " + cardAmount, Logger.LogHandler.LogType.Notify);

                                    cardAmount = viewModel.Amount + viewModel.OpeningBalance;

                                    viewModel.Credit = viewModel.Amount;
                                    Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                                    viewModel.Debit = 0;

                                    // Closing Balance
                                    viewModel.ClosingBalance = cardAmount;
                                    viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId;

                                    viewModel.UserName = user.UserName;
                                    viewModel.Number = user.CustId;
                                    transactionId = _walletTransactionService.AddTransaction(viewModel);
                                    Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                                    _identityManager.UpdateCard(user.Id, cardAmount);
                                    Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                                //---------- DEBIT WALLET ENTRY

                                    walletAmount = user.Wallet;

                                    // Opening Balance
                                    viewModel.Service = ServiceConstants.Debit;
                                    viewModel.OpeningBalance = user.Wallet;
                                    Logger.LogHandler.LogInfo("Opening Balance" + viewModel.OpeningBalance, Logger.LogHandler.LogType.Notify);

                                    walletAmount = Math.Round((double)(walletAmount - viewModel.Amount), 2);

                                    viewModel.Debit = viewModel.Amount;
                                    Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                                    viewModel.Credit = 0;

                                    // Closing Balance
                                    viewModel.ClosingBalance = walletAmount;
                                    viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId;

                                    viewModel.UserName = user.UserName;
                                    transactionId = _walletTransactionService.AddTransaction(viewModel);
                                    Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                                    _identityManager.UpdateWallet(user.Id, walletAmount);
                                    Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                                    scope.Complete();
                                }
                                catch (Exception)
                                {
                                    scope.Dispose();
                                    throw;
                                }
                            }
                            status = "Amount of Rs." + viewModel.Amount + "/- credited to user card (" + user.UserName + ") successfully.";
                        }
                        catch (Exception exception)
                        {
                            status = LogHandler.LogError(exception).Message;
                            bStatus = false;
                        }

                        break;
                }


            return new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount,
                CardBalance = cardAmount
            };
        }

        public ResponseModel CardCredit(RequestModel viewModel)
        {
            var status = "Credit Successful";
            var bStatus = true;
            double? walletAmount = 0;
            var transactionId = 0;

            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                if (viewModel.PaymentMode != PaymentMode.KotakPG)
                    throw new Exception("Invalid payment mode for credit.");

                Logger.LogHandler.LogInfo("User validation", Logger.LogHandler.LogType.Notify);

                // Validate service model
                var user = ValidateServiceViewModel(viewModel);

                Logger.LogHandler.LogInfo("User validation Succesfull" + user.UserName, Logger.LogHandler.LogType.Notify);

                CheckWalletBalanceInLimit(user, viewModel.Amount);  // Check Wallet Limit

                Logger.LogHandler.LogInfo("Payment Mode" + viewModel.PaymentMode, Logger.LogHandler.LogType.Notify);

                var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

                var userName = _prePaidServices.Encrypt(viewModel.UserName);
                var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = userName });
                walletAmount = Convert.ToDouble(responseModel.CardAmount);

                using (var scope = new TransactionScope(TransactionScopeOption.Required,new TransactionOptions{IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout}))
                {
                    try
                    {
                        // Opening Balance
                        viewModel.OpeningBalance = walletAmount;
                        Logger.LogHandler.LogInfo("Opening Balance" + walletAmount, Logger.LogHandler.LogType.Notify);

                        walletAmount = viewModel.Amount + walletAmount;

                        viewModel.Credit = viewModel.Amount;
                        Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                        viewModel.Debit = 0;


                        // Closing Balance
                        viewModel.ClosingBalance = walletAmount;
                        viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId;

                        var safexResult = _prePaidServices.ReflectTransaction(viewModel);
                        if (!safexResult.Status)
                        {
                            throw new Exception("Reflection at safex failed");
                        }

                        viewModel.UserName = user.UserName;
                        transactionId = _walletTransactionService.AddTransaction(viewModel);
                        Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                        _identityManager.UpdateCard(user.Id, walletAmount);
                        Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                        scope.Complete();
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
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0,
                TransactionId = transactionId.ToString().PadLeft(6, '0')
            };

            // Generate Log
            var request = JsonConvert.SerializeObject(viewModel);
            var response = JsonConvert.SerializeObject(walletReturnInfo);
            _logService.GenerateLog(ServiceConstants.Credit, "Web Control Panel", "Credit", request, response, viewModel.UserName);

            return walletReturnInfo;
        }

        public ResponseModel CardDebit(RequestModel viewModel)
        {
            var status = "Debit Successful";
            var bStatus = true;
            double? walletAmount = 0;
            var transactionId = 0;

            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                Logger.LogHandler.LogInfo("User validation", Logger.LogHandler.LogType.Notify);

                // Validate service model
                var user = GetUser(viewModel.UserName, _identityManager);

                Logger.LogHandler.LogInfo("User validation Succesfull" + user.UserName, Logger.LogHandler.LogType.Notify);

                var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

                //var userName = _prePaidServices.Encrypt(viewModel.UserName);
                var responseModel = _prePaidServices.GetWalletBalance(new RequestModel { UserName = viewModel.UserName });
                walletAmount = Convert.ToDouble(responseModel.CardAmount);

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {
                        // Opening Balance
                        viewModel.OpeningBalance = walletAmount;
                        Logger.LogHandler.LogInfo("Opening Balance" + walletAmount, Logger.LogHandler.LogType.Notify);

                        walletAmount = walletAmount - viewModel.Amount;

                        viewModel.Debit = viewModel.Amount;
                        Logger.LogHandler.LogInfo("Amount" + viewModel.Amount, Logger.LogHandler.LogType.Notify);
                        viewModel.Credit = 0;


                        // Closing Balance
                        viewModel.ClosingBalance = walletAmount;
                        viewModel.CyberPlatTransId = string.IsNullOrEmpty(viewModel.CyberPlatTransId) ? "JW" + Guid.NewGuid().ToString() : viewModel.CyberPlatTransId;
                        viewModel.UserName = user.UserName;

                        var safexResult = _prePaidServices.ReflectTransaction(viewModel);
                        if (!safexResult.Status)
                        {
                            throw new Exception("Reflection at safex failed");
                        }

                        
                        transactionId = _walletTransactionService.AddTransaction(viewModel);
                        Logger.LogHandler.LogInfo("Update wallet" + user.Id, Logger.LogHandler.LogType.Notify);
                        _identityManager.UpdateCard(user.Id, walletAmount);
                        Logger.LogHandler.LogInfo("Update wallet successfull", Logger.LogHandler.LogType.Notify);

                        scope.Complete();
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
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var walletReturnInfo = new ResponseModel()
            {
                Status = bStatus,
                Result = status,
                WalletBalance = walletAmount ?? 0,
                TransactionId = transactionId.ToString().PadLeft(6, '0')
            };

            // Generate Log
            var request = JsonConvert.SerializeObject(viewModel);
            var response = JsonConvert.SerializeObject(walletReturnInfo);
            _logService.GenerateLog(ServiceConstants.Credit, "Web Control Panel", "Credit", request, response, viewModel.UserName);

            return walletReturnInfo;
        }

        public IEnumerable<WalletTransaction> GetRecentTransaction(RequestModel viewModel)
        {
            var _walletService = (IWalletService)Bootstrapper.GetService(typeof(WalletService));
            var user = _walletService.GetUser(viewModel.UserName, _identityManager);
            return _walletTransactionService.Get(w => w.UserName == user.UserName && w.Service == viewModel.Service).ToList().OrderByDescending(x => x.TransactionDate).Take(5);
        }
        #endregion
    }
}
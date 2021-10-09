using System;
using System.Collections.Generic;
using System.Transactions;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Newtonsoft.Json.Linq;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers;
using Corno.Services.Bootstrapper;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services
{
    public class CyberPlatGiftCardService : WalletBaseService, ICyberPlatGiftCardService
    {
        #region -- Constructors --
        public CyberPlatGiftCardService(IdentityManager identityManager,
            ICyberPlatApiService cyberPlatApiService, IWalletTransactionService walletTransactionService, ICommissionApiServices commissionService) : base(identityManager)
        {
            _identityManager = identityManager;
            _cyberPlatApiService = cyberPlatApiService;
            _walletTransactionService = walletTransactionService;
            _commissionService = commissionService;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly ICyberPlatApiService _cyberPlatApiService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ICommissionApiServices _commissionService;
        #endregion

        #region -- Methods --

        public ResponseModel Validate(RequestModel viewModel)
        {
            var status = "Validation Successful.";
            var bStatus = true;
            var transactionId = string.Empty;
            var sessionId = string.Empty;
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
                sessionId = Encrypt(transactionId, FieldConstants.SessionIv);
            }
            catch (Exception exception)
            {
                status = LogHandler.LogError(exception).Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                TransactionId = transactionId,
                SessionId = sessionId
            };

            return returnViewModel;
        }

        public ResponseModel Payment(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var transactionId = string.Empty;
            var additionalInfo = string.Empty;
            var user = new ApplicationUser();
            object result = null;
            try
            {
                if (viewModel.Type == "5")
                {
                    // Validate service model
                    user = ValidateServiceViewModel(viewModel);
                }

                // Get number for payment
                ValidateNumber(viewModel);

                // Call cyberplat api to recharge here.
                var response = _cyberPlatApiService.Payment(viewModel);
                viewModel.CyberPlatTransId = response.Between("TRANSID=", Environment.NewLine);
                viewModel.OperatorTransId = response.Between("AUTHCODE=", Environment.NewLine);
                additionalInfo = response.Between("ADDINFO=", Environment.NewLine);//.Replace("\"", "'")

                if(response.Between("ERROR=", Environment.NewLine)!= "0" && response.Between("RESULT=", Environment.NewLine)!="0" && string.IsNullOrEmpty(response.Between("TRNXSTATUS=", Environment.NewLine)))
                {
                    throw new Exception("Transaction Failed. Please contact : 022-28373737 for futher assistance");
                }

                if (viewModel.Type == "5")
                {

                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions
                            {
                                IsolationLevel = IsolationLevel.ReadCommitted,
                                Timeout = TransactionManager.MaximumTimeout
                            }))
                    {
                        try
                        {

                            //Save Required parameters
                            viewModel.Response = additionalInfo;

                            // Opening Balance
                            viewModel.OpeningBalance = user.Wallet;

                            var walletAmount = user.Wallet;

                            // Update wallet if payment mode is wallet
                            if (string.Equals(viewModel.PaymentMode, PaymentMode.Wallet,
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                walletAmount = walletAmount - viewModel.Amount;
                                viewModel.Credit = 0;
                                viewModel.Debit = viewModel.Amount;
                            }

                          //  var comissionOperator = _cyberPlatApiService.GetCommissionOperator(viewModel.Service, viewModel.Operator);
                            // Add commision
                           // viewModel.Commission = _commissionService.GetCommission(viewModel.Service, comissionOperator.Trim(), viewModel.Amount, user);
                            //_cyberPlatApiService.GetCommission(viewModel.Service,
                            //    viewModel.Operator, user.UserType);


                           // if (viewModel.Commission > 0)
                           //  walletAmount += viewModel.Commission;// / 100 * viewModel.Amount;

                            _identityManager.UpdateWallet(user.Id, walletAmount);

                            // Closing Balance
                            viewModel.ClosingBalance = walletAmount;
                            viewModel.UserName = user.UserName;

                            //if (user.CustId != null && viewModel.PaymentMode.ToLower() == PaymentMode.Wallet.ToLower())
                            //{
                            //    var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

                            //    viewModel.Amount = Convert.ToDouble(viewModel.Debit - viewModel.Credit);
                            //    var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                            //    if (!safexResult.Status)
                            //    {
                            //        throw new Exception("Reflection at safex failed");
                            //    }

                            //}

                            // Create Wallet Transaction
                            _walletTransactionService.AddTransaction(viewModel);

                            // Send IMPS transaction id to user
                            transactionId = viewModel.OperatorTransId;
                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            scope.Dispose();
                            throw;
                        }
                    }
                }

                switch (viewModel.Type)
                {
                    case "1":
                        {
                            var data = new List<Tuple<string, string>>(); // or, List<YourClass>
                            var lines = additionalInfo.Split('\n');
                            foreach (var line in lines)
                            {
                                var values = line.Split(';');
                                if (values.Length < 2) continue;
                                data.Add(new Tuple<string, string>(values[0], values[1]));
                                // or, populate YourClass          
                            }
                            additionalInfo = new JavaScriptSerializer().Serialize(data);
                            result = JsonConvert.DeserializeObject(additionalInfo);
                        }
                        break;
                    case "3":
                        additionalInfo = additionalInfo.Replace("'s Day", "s Day");
                        result = JsonConvert.DeserializeObject(additionalInfo);
                        break;
                    default:
                        additionalInfo = additionalInfo.Replace("'s", "s");
                        result = JsonConvert.DeserializeObject(additionalInfo);
                        break;
                }

            }
            catch (Exception exception)
            {
                status = exception.Message;
                if (null != exception.InnerException)
                {
                    var message = exception.InnerException.Message;
                    additionalInfo = message.Between("ADDINFO=", Environment.NewLine);
                }
                bStatus = false;
                if (viewModel.Type == "5")
                {
                    var customerMessage = "Transaction for " + viewModel.Service + " (" + viewModel.Number + ") " + "has failed. If Rs." + viewModel.Amount + " is deducted, it will be refunded within 6-7 working days.";
                    TboBaseController.SendSms(viewModel.UserName, customerMessage, string.Empty);
                }
            }
            
            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                WalletBalance = 0,
                TransactionId = transactionId,
                AddionalInfo = additionalInfo,
                Response = result
            };

            return returnViewModel;
        }
        #endregion
    }
}
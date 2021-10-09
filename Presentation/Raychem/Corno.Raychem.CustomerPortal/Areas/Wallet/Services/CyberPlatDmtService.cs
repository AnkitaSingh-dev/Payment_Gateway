using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Corno.Data.Helpers;
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
    public class CyberPlatDmtService : WalletBaseService, ICyberPlatDmtService
    {
        #region -- Constructors --
        public CyberPlatDmtService(IdentityManager identityManager,
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
            var dataHtml = string.Empty;
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
                dataHtml = response.Between("DATA_HTML=", "END");
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
                SessionId = sessionId,
                DataHtml = System.Web.HttpUtility.UrlEncode(dataHtml)
            };

            return returnViewModel;
        }

        public ResponseModel Payment(RequestModel viewModel)
        {
            var status = "Transaction Successful.";
            var bStatus = true;
            var transactionId = string.Empty;
            var addionalInfo = string.Empty;
            var dataHtml = string.Empty;
            var limit = 0.0; var user = new ApplicationUser();
            try
            {
                if ((viewModel.Type == "3" && viewModel.Amount < 100) || viewModel.Amount > 5000)
                    throw new Exception("Invalid amount, should be more than 100 and less than 5000");

                var comissionOperator = _cyberPlatApiService.GetCommissionOperator(viewModel.Service, viewModel.Operator);
                var conveninceCharges = comissionOperator == string.Empty ? 0 : _commissionService.GetConvinienceCharges(viewModel.Service, comissionOperator.Trim(), viewModel.Amount);

                viewModel.AmountAll = (viewModel.Amount + 6).ToString();
                conveninceCharges = (conveninceCharges) > 20 ? conveninceCharges : 20;
                viewModel.Price = conveninceCharges.ToString();

                if (viewModel.Type == "9")
                {
                    user = GetUser(viewModel.UserName, _identityManager);
                }
                else
                {
                    // Validate service model
                    user = ValidateServiceViewModel(viewModel);
                }

                //if (!user.IsPanSubmit && viewModel.Type == "9")
                //{
                //    throw new Exception("PAN card is mandatory for DMT service, Please complete KYC to avail this facility.");
                //}

                // Get number for payment
                ValidateNumber(viewModel);

                var validTransactionDate = _walletTransactionService.Get(w => w.UserName == user.UserName && w.Service.ToUpper() == ServiceConstants.Dmt.ToUpper()).ToList().Max(x => x.TransactionDate);
                if (DateTime.Now.Subtract(Convert.ToDateTime(validTransactionDate)).TotalSeconds < 60)
                    throw new Exception("There should be atleast a minute's interval between two transactions.");

                if (user.Wallet < (viewModel.Amount + conveninceCharges) && viewModel.Type != "3")
                    throw new Exception("Wallet does not have sufficient balance");

                // Call cyberplat api to recharge here.
                var response = _cyberPlatApiService.Payment(viewModel);

                // dataHtml = response.Between("DATA_HTML=", Environment.NewLine);
                viewModel.CyberPlatTransId = response.Between("TRANSID=", Environment.NewLine);
                viewModel.OperatorTransId = response.Between("AUTHCODE=", Environment.NewLine);
                addionalInfo = response.Between("ADDINFO=", Environment.NewLine).Replace("\"", "'");

                if (viewModel.Type == "5" && viewModel.Operator == "yesBank")
                {
                    limit = Convert.ToDouble(addionalInfo.Between("'SENDER_AVAILBAL':", ","));

                    if (limit == 0.0 || limit <= 1000.0)
                    {
                        addionalInfo = string.Empty;
                        throw new Exception("You have exhausted your monthly limit of ₹25,000, available limit : " + limit);
                    }
                }

                if (viewModel.Type == "3") // && (response.Between("TRNXSTATUS=", Environment.NewLine) != "3")
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
                            viewModel.Pnr = viewModel.BeneficiaryName;
                            viewModel.BookingId = viewModel.BeneficiaryAccount;
                            // Temporary
                            viewModel.PaymentMode = PaymentMode.Wallet;

                            // Opening Balance
                            viewModel.OpeningBalance = user.Wallet;

                            var walletAmount = user.Wallet;

                            // Add commision
                            viewModel.Commission = _commissionService.GetCommissionDmt(viewModel.Amount, user);

                            viewModel.Credit = viewModel.Commission;
                            viewModel.Debit = Math.Round(viewModel.Amount + conveninceCharges, 4);
                            viewModel.Amount = viewModel.Amount;
                            walletAmount = walletAmount - viewModel.Debit + viewModel.Commission;


                            _identityManager.UpdateWallet(user.Id, walletAmount);

                            viewModel.UserName = user.UserName;

                            // Closing Balance
                            viewModel.ClosingBalance = viewModel.OpeningBalance - viewModel.Debit + viewModel.Commission;

                            // Send IMPS transaction id to user
                            transactionId = viewModel.CyberPlatTransId;

                            //var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));

                            //if (user.CustId != null && viewModel.PaymentMode.ToLower() == PaymentMode.Wallet.ToLower())
                            //{
                            //    viewModel.Amount = Convert.ToDouble(viewModel.Debit - viewModel.Credit);

                            //    var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                            //    if (!safexResult.Status)
                            //    {
                            //        throw new Exception("Reflection at safex failed");
                            //    }
                            //}

                            // Create Wallet Transaction
                            _walletTransactionService.AddTransaction(viewModel);

                            scope.Complete();
                        }
                        catch (Exception)
                        {
                            scope.Dispose();
                            throw;
                        }
                    }
                }

                //else if (response.Between("TRNXSTATUS=", Environment.NewLine) == "3")
                //{
                //   var result = DMTTransaction(viewModel);
                //    bStatus = false;
                //    status = result.Result;
                //    transactionId = result.TransactionId;
                //}
            }

            catch (Exception exception)
            {
                bStatus = false;
                status = exception.Message;

                if (null != exception.InnerException)
                {
                    status = "Error in transaction from bank, Please try again later or contact : 022-28373737 for futher assistance ";
                    var response = exception.InnerException.Message;

                    if (response.Between("ERROR=", Environment.NewLine) != "0")
                    {
                        status = string.IsNullOrEmpty(response.Between("ERRMSG=", Environment.NewLine)) ?
                            string.IsNullOrEmpty(response.Between("errmsg=", Environment.NewLine)) ? status :
                            response.Between("errmsg=", Environment.NewLine) : response.Between("ERRMSG=", Environment.NewLine);
                    }

                    addionalInfo = string.IsNullOrEmpty(response.Between("ADDINFO=", Environment.NewLine)) ? status : response.Between("ADDINFO=", Environment.NewLine);
                }
                if (viewModel.Type == "3")
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
                AddionalInfo = addionalInfo,
                DataHtml = dataHtml
            };

            return returnViewModel;
        }

        public ResponseModel IsUserValidAgent (RequestModel viewModel)
        {
            var status = "Validation Successful.";
            var bStatus = true;
            var agentId = string.Empty;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var user = GetUser(viewModel.UserName, _identityManager); //_identityManager.GetUserFromUserName(viewModel.UserName);
                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                var userDMTAgentRT = (IUserDMTAgentRTService)Bootstrapper.GetService(typeof(UserDMTAgentRTService));
                var isExist = userDMTAgentRT.Get(w => w.UserName == user.UserName).FirstOrDefault();
                if (isExist == null)
                {
                    bStatus = false;
                    throw new Exception("Invalid Agent");
                }
                else
                    agentId = isExist.AgentId;
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
                AgentId = agentId
            };

            return returnViewModel;
        }
        public RequestModel GetTransactionDetail (string transactionId)
        {
            var response = _walletTransactionService.GetById(int.Parse(transactionId));
            if (response == null)
                throw new Exception("This Transaction doesnt exist");

            var _userDMTAgentRT = (IUserDMTAgentRTService)Bootstrapper.GetService(typeof(UserDMTAgentRTService));
            var agentId = _userDMTAgentRT.Get(x => x.UserName == response.UserName).ToList().FirstOrDefault().AgentId;
            return new RequestModel
            {
                AgentId = agentId,
                BeneficiaryAccount = response.BookingId,
                BeneficiaryName = response.Pnr,
                CyberPlatTransId = response.CyberPlatTransId,
                Operator = response.Operator,
                Amount = response.Amount,
                Service = response.Service,
                Commission = response.Commission,
                Debit = response.Debit,
                UserName = response.UserName,
                FromDate = response.TransactionDate.ToString(),
                State = response.Status,
                TransactionId = response.Id.ToString().PadLeft(6, '0')
            };
        }

        public ResponseModel Refund(RequestModel viewModel)
        {
            var status = "Refund Successful.";
            var bStatus = true;
            var addionalInfo = string.Empty;
            try
            {
                // Validate service model
                var user = GetUser(viewModel.UserName, _identityManager);

                if (null == user)
                    throw new Exception(viewModel.UserName + "'s Account does not exist!");

                // Get number for payment
                ValidateNumber(viewModel);

                // Call cyberplat api to recharge here.
                var response = _cyberPlatApiService.Payment(viewModel);

                if (response.Between("ERROR=", Environment.NewLine) != "0")
                {
                    throw new Exception(response.Between("errmsg=", Environment.NewLine));
                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions{IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout}))
                {
                    try
                    {
                        var record = _walletTransactionService.Get(x => x.CyberPlatTransId == viewModel.TransactionId).ToList().FirstOrDefault();

                        if (response == null)
                            throw new Exception("This Transaction doesnt exist");

                        var walletAmount = user.Wallet;

                        // Add commision
                        record.Commission = 0;

                        var temp = record.Credit;
                        record.Credit = record.Debit ;
                        record.Debit = temp;
                        record.Status = StatusConstants.Refunded;

                        walletAmount = walletAmount - record.Debit + record.Credit;

                        _identityManager.UpdateWallet(user.Id, walletAmount);

                        // Closing Balance
                        record.ClosingBalance = walletAmount;

                        // Create Wallet Transaction
                        _walletTransactionService.Update(record);
                        _walletTransactionService.Save();

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
                status = exception.Message;
                bStatus = false;
            }

            var returnViewModel = new ResponseModel
            {
                Status = bStatus,
                Result = status,
                TransactionId = viewModel.CyberPlatTransId,
                AddionalInfo = addionalInfo
            };

            return returnViewModel;
        }

        private ResponseModel DMTTransaction(RequestModel viewModel)
        {
            var addInfo = string.Empty;
            var status = string.Empty;
            var transactionId = string.Empty;

            viewModel.FromDate = new DateTime().Date.ToString();
            viewModel.ToDate = DateTime.Now.Date.ToString();
            viewModel.Type = "14";

            try
            {
                // Call cyberplat api to recharge here.
                var response = _cyberPlatApiService.Payment(viewModel);

                var error = response.Between("errmsg=", Environment.NewLine);

                if (string.IsNullOrEmpty(error))
                {
                    addInfo = response.Between("ADDINFO=", "]}}") + "]}}";
                    var DmtDetails = JsonConvert.DeserializeObject<DMTTransactionModel>(addInfo);

                    foreach (var detail in DmtDetails.DATA.TRANSACTION_DETAILS)
                    {
                        if (viewModel.CyberPlatTransId == detail.CUSTOMER_REFERENCE_NO) // && detail.TRANSACTION_STATUS == "FAILED"
                        {
                            var userName = Encrypt(viewModel.UserName, FieldConstants.LoginIv);
                            var user = GetUser(userName, _identityManager);

                            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                            {
                                try
                                {
                                    viewModel.Pnr = viewModel.BeneficiaryName;
                                    viewModel.BookingId = viewModel.BeneficiaryAccount;
                                    // Temporary
                                    viewModel.PaymentMode = PaymentMode.Wallet;

                                    // Opening Balance
                                    viewModel.OpeningBalance = user.Wallet;

                                    var walletAmount = user.Wallet;

                                    // Add commision
                                    viewModel.Commission = _commissionService.GetCommissionDmt(viewModel.Amount, user);

                                    viewModel.Credit = viewModel.Commission;

                                    var conveninceCharges = Convert.ToDouble(viewModel.Price);

                                    viewModel.Debit = Math.Round(viewModel.Amount + conveninceCharges, 4);
                                    viewModel.Amount = viewModel.Amount;

                                    walletAmount = walletAmount - viewModel.Debit + viewModel.Commission;
                                    _identityManager.UpdateWallet(user.Id, walletAmount);

                                    // Closing Balance
                                    viewModel.ClosingBalance = viewModel.OpeningBalance - viewModel.Debit + viewModel.Commission;
                                    viewModel.State = detail.TRANSACTION_STATUS == "FAILED" ? StatusConstants.Failure : StatusConstants.Success;

                                    // Create Wallet Transaction
                                    _walletTransactionService.AddTransaction(viewModel);

                                    // Send IMPS transaction id to user
                                    transactionId = viewModel.CyberPlatTransId;

                                    if (user.CustId != null)
                                    {
                                        var _prePaidServices = (IFederalPrePaidCardService)Bootstrapper.GetService(typeof(FederalPrePaidCardService));
                                        var safexResult = _prePaidServices.ReflectTransaction(viewModel);

                                        if (!safexResult.Status)
                                        {
                                            throw new Exception("Reflection at safex failed");
                                        }
                                    }
                                    scope.Complete();
                                    status = "Transaction has failed, Please request for refund from My Orders section";
                                }
                                catch (Exception)
                                {
                                    scope.Dispose();
                                    throw;
                                }
                            }
                        }

                        else
                            status = "Transaction has failed, Please try after sometime";

                    }
                }
            }

            catch (Exception e)
            {
                throw new Exception("Bank servers are currently unoperative");
            }
            return new ResponseModel {
                Result = status,
                TransactionId = transactionId
            };
        }

        #endregion
    }
}
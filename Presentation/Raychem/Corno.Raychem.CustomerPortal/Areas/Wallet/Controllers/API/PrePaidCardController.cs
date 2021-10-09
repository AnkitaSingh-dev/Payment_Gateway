 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Corno.Services.Encryption.Interfaces;
using Corno.Services.Encryption;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using System.Net.Http.Formatting;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Logger;
using Newtonsoft.Json;
using Corno.Services.Bootstrapper;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers.API
{
    public class PrePaidCardController : ApiController
    {
        #region -- Constructors --

        public PrePaidCardController(IPrePaidCardService PrePaidCardService)
        {
            _prePaidCardService = PrePaidCardService;
        }
        #endregion

        #region -- Data Members --
        private readonly IPrePaidCardService _prePaidCardService;
        #endregion

        #region -- Methods --

        public HttpResponseMessage CreateUser(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.CreateUser(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage MobileOTPVerification(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.MobileOTPVerification(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage AadhaarRegistration(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.AadhaarRegistration(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage PanRegistration(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.PanRegistration(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        //public HttpResponseMessage VerifyAadhaarOTP(RequestModel viewModel)
        //{
        //    var responseModel = _prePaidCardService.VerifyAadhaarOTP(viewModel);
        //    return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        //}

        public HttpResponseMessage FetchCardUserProfile(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.FetchCardUserProfile(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchBankWalletKYCStatus(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.FetchBankWalletKYCStatus(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage BlockUnblockCloseCardWallet(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.BlockUnblockCloseCardWallet(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage SubmitFullKYCRequest(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.SubmitFullKYCRequest(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage AddBeneficiary(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.AddBeneficiary(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchBeneficiary(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.FetchBeneficiary(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        //public HttpResponseMessage SmartCardUserList(RequestModel viewModel)
        //{
        //    var responseModel = _prePaidCardService.SmartCardUserList(viewModel);
        //    return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        //}

        //public HttpResponseMessage CardDetails(RequestModel viewModel)
        //{
        //    var responseModel = _prePaidCardService.GetCardDetails(viewModel);
        //    return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        //}

        public HttpResponseMessage FetchCardTransactions(RequestModel viewModel)
        {
            string status;
            double? walletAmount = 0;
            try
            {
                if (null == viewModel)
                    throw new Exception("Bad Request");

                var _walletService = (IWalletService)Bootstrapper.GetService(typeof(WalletService));
                var _identityManager = (IdentityManager)Bootstrapper.GetService(typeof(IdentityManager));

                var user = _walletService.GetUser(viewModel.UserName, _identityManager);
                if (null == user)
                {
                    var userName = _walletService.Decrypt(viewModel.UserName, FieldConstants.LoginIv);
                    throw new Exception("User (" + userName + ") does not exist in the system.");
                }

                var safexWalletTxnService = (ISafexWalletTransactionService)Bootstrapper.GetService(typeof(SafexWalletTransactionService));

                var transaction = safexWalletTxnService.Get(w => w.UserName == user.UserName)
                    .Select(w => new
                    {
                        TransactionId = w.WalletTransactionId.ToString().PadLeft(6, '0'),
                        w.UserName,
                        TransactionDate = w.CreatedDate?.ToString("dd/MM/yyyy"),
                        CyberPlatTransId = w.SafexRefId,
                        OperatorTransId = w.SwitchRefId,
                        w.Amount,
                        Commission = 0.0,
                        OpeningBalance = 0.0,
                        Credit = 0.0,
                        Debit = w.Amount,
                        ClosingBalance = 0.0,
                        w.Status
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

        public HttpResponseMessage UpdateAadhaar(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.UpdateAadhaar(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage IsCardUser(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.IsCardUser(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage ReflectTransaction(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.ReflectTransaction(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage AddPhyscicalCard(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.AddPhyscicalCard(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage PhyscialCardLUB(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.PhyscialCardLUB(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage PhysicalCardSetPin(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.PhysicalCardSetPin(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchVirtualCard(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.FetchVirtualCard(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage BlockVirtualCard(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.BlockVirtualCard(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage GetCheckPartnerBalance()
        {
            var responseModel = _prePaidCardService.GetCheckPartnerBalance();
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage ReQuery(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.ReQuery(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage TransactionHistory(RequestModel viewModel)
        {
            var responseModel = _prePaidCardService.TransactionHistory(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public string EncryptUsername(string number)
        {
            return _prePaidCardService.Encrypt(number);
        }

        public string DecryptUsername(string number)
        {
            return _prePaidCardService.Decrypt(number);
        }
        #endregion

    }
}

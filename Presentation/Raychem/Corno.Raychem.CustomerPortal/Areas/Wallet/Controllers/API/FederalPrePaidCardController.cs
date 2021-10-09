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
    public class FederalPrePaidCardController : ApiController
    {
        #region -- Constructors --

        public FederalPrePaidCardController(IFederalPrePaidCardService FederalPrePaidCardService)
        {
            _federalPrePaidCardService = FederalPrePaidCardService;
        }
        #endregion

        #region -- Data Members --
        private readonly IFederalPrePaidCardService _federalPrePaidCardService;
        #endregion

        #region -- Methods --

        public HttpResponseMessage UserRegistration(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.UserRegistration(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage SCRegistration(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.SCRegistration(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage UserCardList(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.UserCardList(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage CardLUB(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.CardLUB(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage ResetPin(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.ResetPin(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage UserCardData(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.UserCardData(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage WalletBalance(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.GetWalletBalance(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage ShowUserProfile(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.ShowUserProfile(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage ReflectTransaction(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.ReflectTransaction(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage SubmitFullKYC(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.SubmitFullKYC(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchCardTransactions(RequestModel viewModel)
        {
            var responseModel = _federalPrePaidCardService.FetchCardTransactions(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public string EncryptUsername(string number)
        {
            return _federalPrePaidCardService.Encrypt(number);
        }
        #endregion

    }
}

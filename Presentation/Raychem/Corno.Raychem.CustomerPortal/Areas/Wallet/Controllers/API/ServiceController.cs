using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using Corno.Logger;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class ServiceController : ApiController
    {
        #region -- Constructors --

        public ServiceController(ICyberPlatUrlService cyberPlatUrlService, ICyberPlatService cyberPlatService)
        {
            _cyberPlatUrlService = cyberPlatUrlService;
            _cyberPlatService = cyberPlatService;
        }
        #endregion

        #region -- Data Members --
        private readonly ICyberPlatUrlService _cyberPlatUrlService;
        private readonly ICyberPlatService _cyberPlatService;
        #endregion

        #region -- Methods --

        [HttpGet]
        public HttpResponseMessage GetOperators(string service)
        {
            try
            {
                // Get Operators List
                var operators = _cyberPlatUrlService.Get(c => c.Service == service)
                    .Select(c => new
                    {
                        c.Service,
                        c.Operator
                    });

                return Request.CreateResponse(HttpStatusCode.OK, operators.OrderBy(x => x.Operator));
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, LogHandler.LogError(exception).Message);
            }
        }

        public HttpResponseMessage FetchPlans(RequestModel viewModel)
        {
            return Validate(viewModel);
        }

        public HttpResponseMessage Validate(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.Validate(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage Payment(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.Payment(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage GetBalance()
        {
            var responseModel = _cyberPlatService.GetBalance();
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage MposTransactionCapture(MposResponseModel viewModel)
        {
            _cyberPlatService.MposTransactionCapture(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpPost]
        public HttpResponseMessage GetRSA(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.GetRSA(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage SaveFCMToken(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.SaveFCMToken(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchElectricityBillDetails(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.FetchElectricityBillDetails(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchOperator(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.FetchOperator(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage FetchPrepaidPlans(RequestModel viewModel)
        {
            var responseModel = _cyberPlatService.FetchPrepaidPlans(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        #endregion
    }
}
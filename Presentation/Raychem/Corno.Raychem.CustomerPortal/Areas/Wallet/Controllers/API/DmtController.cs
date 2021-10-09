using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class DmtController : ApiController
    {
        #region -- Constructors --

        public DmtController(ICyberPlatDmtService cyberPlatDmtService)
        {
            _cyberPlatDmtService = cyberPlatDmtService;
        }
        #endregion

        #region -- Data Members --
        private readonly ICyberPlatDmtService _cyberPlatDmtService;
        #endregion

        #region -- Methods --
        // GET api/<controller>
        public string Get()
        {
            return "Dmt";
        }
        public HttpResponseMessage Validate(RequestModel viewModel)
        {
             // current time is between start and stop
             var responseModel = _cyberPlatDmtService.Validate(viewModel);
             return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage Payment(RequestModel viewModel)
        {
                var responseModel = _cyberPlatDmtService.Payment(viewModel);
                return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }


        public HttpResponseMessage IsUserValidAgent (RequestModel viewModel)
        {
            var responseModel = _cyberPlatDmtService.IsUserValidAgent(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage GetTransactionDetail(string transactionId)
        {
            var responseModel = _cyberPlatDmtService.GetTransactionDetail(transactionId);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage Refund(RequestModel viewModel)
        {
            var responseModel = _cyberPlatDmtService.Refund(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }
        #endregion
    }
}
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class CyberPlatGiftCardController : ApiController
    {
        #region -- Constructors --

        public CyberPlatGiftCardController(CyberPlatGiftCardService cyberPlatGiftCardService)
        {
            _cyberPlatGiftCardService = cyberPlatGiftCardService;
        }
        #endregion

        #region -- Data Members --
        private readonly CyberPlatGiftCardService _cyberPlatGiftCardService;
        #endregion

        #region -- Methods --
        // GET api/<controller>
        public string Get()
        {
            return "GiftCard";
        }

        public HttpResponseMessage Payment(RequestModel viewModel)
        {
            var responseModel = _cyberPlatGiftCardService.Payment(viewModel);
            return Request.CreateResponse(HttpStatusCode.OK, responseModel, JsonMediaTypeFormatter.DefaultMediaType);
        }

        #endregion
    }
}
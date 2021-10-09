using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using System.Web;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Services.Bootstrapper;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class GeneralController : ApiController
    {
        #region -- Constructors --

        public GeneralController(StateCityUTService stateCityUTService)
        {
            _stateCityUTService = stateCityUTService;
        }
        #endregion

        #region -- Data Members --
        private readonly StateCityUTService _stateCityUTService;
        #endregion

        #region -- Methods --

        public HttpResponseMessage GetState()
        {
            var stateList = _stateCityUTService.Get().OrderBy(a => a.State).Select(a => a.State).Distinct().ToList();
            return Request.CreateResponse(HttpStatusCode.OK, stateList, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage GetCity(string state)
        {
            var cityList = _stateCityUTService.Get(a => a.State == state).OrderBy(a => a.City).Select(a => a.City).Distinct().ToList();
            return Request.CreateResponse(HttpStatusCode.OK, cityList, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage GetFederalState()
        {
            var stateList = _stateCityUTService.Get().OrderBy(a => a.FederalState).Select(a => a.FederalState).Distinct().ToList();
            return Request.CreateResponse(HttpStatusCode.OK, stateList, JsonMediaTypeFormatter.DefaultMediaType);
        }

        public HttpResponseMessage GetFederalCity(string state)
        {
            var cityList = _stateCityUTService.Get(a => a.FederalState == state).OrderBy(a => a.City).Select(a => a.City).Distinct().ToList();
            return Request.CreateResponse(HttpStatusCode.OK, cityList, JsonMediaTypeFormatter.DefaultMediaType);
        }

        #endregion
    }
}

using System;
using System.Web.Mvc;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Controllers;
using Corno.Services.Bootstrapper;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class NotificationController : BaseController
    {
        #region -- Constructors -- 
        public NotificationController(IWalletService walletService, IdentityManager identityManager, ICyberPlatService cyberPlatService)
        {
            _walletService = walletService;
            _identityManager = identityManager;
            _cyberPlatService = cyberPlatService;
        }
        #endregion

        #region -- Data Members --
        private readonly IdentityManager _identityManager;
        private readonly IWalletService _walletService;
        private readonly ICyberPlatService _cyberPlatService;
        #endregion

        #region -- Methods --


        #endregion

        // GET: /Notification/Create
        public ActionResult Create()
        {
            return View(new Notification());
        }

        // POST: /Notification/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Notification model, string sendType)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
               var result = _cyberPlatService.Notification(model, sendType);
                if(result != true)
                {
                    throw new Exception("Notifications Sending Error!!!...");
                }
                TempData["Success"] = StatusConstants.Success;
            }
            catch (Exception exception)
            {
                TempData["Success"] = null;
                HandleControllerException(exception);
            }

            return View(model);
        }
    }
}

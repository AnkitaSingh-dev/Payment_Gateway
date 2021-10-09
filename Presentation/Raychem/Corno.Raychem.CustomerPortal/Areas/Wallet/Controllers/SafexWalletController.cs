using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Controllers;
using Microsoft.AspNet.Identity;
using Corno.Data.Base;
using Corno.Raychem.CustomerPortal.Contexts;
using System.Globalization;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class SafexWalletController : BaseController
    {
        #region --Constructor--
        public SafexWalletController(ISafexWalletTransactionService safexWalletTransactionService, IJugadUserSafexUserService jugadUserSafexUserService)

        {
            _safexWalletTransactionService = null;
            _safexWalletTransactionService = safexWalletTransactionService;
            _jugadUserSafexUserService = null;
            _jugadUserSafexUserService = jugadUserSafexUserService;
        }
        #endregion
        #region -- Data Member --
        private readonly ISafexWalletTransactionService _safexWalletTransactionService;
        private readonly IJugadUserSafexUserService _jugadUserSafexUserService;
        #endregion

        // GET: Wallet/SafexWallet
        public ActionResult Index()
        {
            var model = _safexWalletTransactionService.Get().ToList();
            return View(model);
        }

        // GET: Wallet/SafexWallet/GetSafexUserDetails
        public ActionResult GetSafexUserDetails()
        {
            var model = _jugadUserSafexUserService.Get().ToList();
            return View(model);            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Controllers;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class MyBookingsController : BaseController
    {
        public MyBookingsController(IWalletTransactionService walletTransactionService)
        {
            _walletTransactionService = walletTransactionService;
        }

        #region -- Data Members --
        private readonly IWalletTransactionService _walletTransactionService;
        #endregion

        #region -- Constructors --

        #endregion

        #region -- Actions --

        public ActionResult Index(int? page)
        {           
            var models = new List<WalletTransaction>();
            try
            {
                models = User.IsInRole(Globals.Constants.RoleNames.Admin) || User.IsInRole(Globals.Constants.RoleNames.Viewer)
                    ? _walletTransactionService.Get().Where(t => t.TransactionDate >= Convert.ToDateTime(DateTime.Now.Date) && t.TransactionDate < Convert.ToDateTime(DateTime.Now.Date).AddDays(1)).OrderByDescending(w => w.Id).ToList()
                    : _walletTransactionService.Get(w => w.UserName == User.Identity.Name).Where(t => t.TransactionDate >= Convert.ToDateTime(DateTime.Now.Date) && t.TransactionDate < Convert.ToDateTime(DateTime.Now.Date).AddDays(1))
                        .OrderByDescending(w => w.Id).ToList();
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View(models);
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            string fromDate = form["fromText"].ToString();
            string toDate = form["toText"].ToString();
            var models = new List<WalletTransaction>();            
            try
            {
                if (fromDate != "" && toDate != "")
                {
                    models = User.IsInRole(Globals.Constants.RoleNames.Admin) || User.IsInRole(Globals.Constants.RoleNames.Viewer)
                        ? _walletTransactionService.Get().Where(t => t.TransactionDate >= Convert.ToDateTime(fromDate) && t.TransactionDate < Convert.ToDateTime(toDate).AddDays(1)).OrderByDescending(w => w.Id).ToList()
                        : _walletTransactionService.Get(w => w.UserName == User.Identity.Name).Where(t => t.TransactionDate >= Convert.ToDateTime(fromDate) && t.TransactionDate < Convert.ToDateTime(toDate).AddDays(1))
                            .OrderByDescending(w => w.Id).ToList();
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View(models);
        }

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                    throw new Exception("This is an invalid User ID.");

                var model = _walletTransactionService.GetById(id);
                return View(model);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }

        #endregion
    }
}
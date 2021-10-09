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
//using IronPdf;


namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class CommissionAllController : BaseController
    {
        #region --Constructor--
        //public CommissionAllController(IMasterCommissionService masterCommissionService)

        //{
        //    _masterCommissionService = null;
        //    _masterCommissionService = masterCommissionService;
        //}
        public CommissionAllController(IMasterTransactionService masterTransactionService)
        {
            _masterTransactionService = null;
            _masterTransactionService = masterTransactionService;
        }
        #endregion

        #region -- Data Member --
        //private readonly IMasterCommissionService _masterCommissionService;        

        private readonly IMasterTransactionService _masterTransactionService;
        #endregion


        // GET: Wallet/CommissionAll
        //public ActionResult Index(int? page)
        //{
        //    var models = _masterTransactionService.Get().ToList();
        //    return View(models);
        //    _masterCommissionService.GetCommission();
        //    return View();
        //}

        public ActionResult GetCommissionDetails(int? page)
        {
            var models = _masterTransactionService.Get().ToList();            
            return View(models);  
        }

        public ActionResult EditMasterCommission(int id)
        {
            var models = _masterTransactionService.GetById(id);
            return View(models);         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMasterCommission(MasterTransaction model,int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _masterTransactionService.Update(model);
                    _masterTransactionService.Save();
                }
                else
                    return View(model);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return RedirectToAction("GetCommissionDetails", model);            
        }
    }
}
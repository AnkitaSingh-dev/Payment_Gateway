//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Web.Mvc;
//using Corno.Data.Common;
//using Corno.Globals.Constants;
//using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
//using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
//using Corno.Raychem.CustomerPortal.Controllers;
//using Corno.Services.Common.Interfaces;
//using Microsoft.AspNet.Identity;

//namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
//{
//    public class CommissionController : BaseController
//    {
//        #region -- constructors --
//        public CommissionController(ICommissionService commissionService, IMasterTransactionService masterTransactionService)
//        {
//            _commissionService = commissionService;
//            _masterTransactionService = masterTransactionService;
//        }
//        #endregion

//        #region -- Data Mamber--
//        private readonly ICommissionService _commissionService;
//        private readonly IMasterTransactionService _masterTransactionService;
//        #endregion

//        #region -- Methods --

//        private IEnumerable<MasterTransaction> GetMisMasters()
//        {
//            return _masterTransactionService.Get(m =>
//                m.Service == FieldConstants.Service || m.Operator == FieldConstants.Operator).ToList();
//        }
//        #endregion

//        #region -- Actions --

//        //GET: /Commission/
//        //public ActionResult Index(int? page)
//        //{
//        //    try
//        //    {
//        //        var commissionList = _commissionService.Get().ToList();
//        //        foreach (var commission in commissionList)
//        //        {
//        //            commission.ServiceName = _masterTransactionService.GetName(commission.ServiceId ?? 0);
//        //            commission.OperatorName = _masterTransactionService.GetName(commission.OperatorId ?? 0);
//        //        }

//        //        return View(commissionList);
//        //    }
//        //    catch (Exception exception)
//        //    {
//        //        HandleControllerException(exception);
//        //    }
//        //    return View();
//        //}

//        // GET: /CCustomerMaster/Details/5
//        public ActionResult Details(int? id)
//        {
//            try
//            {
//                if (id == null)
//                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//                var commission = _commissionService.GetById(id);
//                if (commission == null)
//                    return HttpNotFound();

//                return View(commission);
//            }
//            catch (Exception exception)
//            {
//                HandleControllerException(exception);
//            }
//            return View();
//        }

//        //// GET: /CCustomerMaster/Create
//        //public ActionResult Create()
//        //{
//        //    try
//        //    {
//        //        var model = new Commission
//        //        {
//        //            MasterTransactions = GetMisMasters()
//        //        };

//        //        return View(model);
//        //    }
//        //    catch (Exception exception)
//        //    {
//        //        HandleControllerException(exception);
//        //    }
//        //    return View();
//        //}

//        // POST: /CCustomerMaster/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Commission model)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {

//                    _commissionService.Add(model);
//                    _commissionService.Save();
//                    return RedirectToAction("Index");
//                }
//                return View(model);
//            }
//            catch (Exception exception)
//            {
//                HandleControllerException(exception);
//            }
//            return View();
//        }

//        //// GET: /CCustomerMaster/Edit/5
//        //public ActionResult Edit(int? id)
//        //{
//        //    try
//        //    {
//        //        if (id == null)
//        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//        //        var model = _commissionService.GetById(id);
//        //        if (model == null)
//        //            return HttpNotFound();
//        //        model.MiscMasters = GetMisMasters();

//        //        return View(model);
//        //    }
//        //    catch (Exception exception)
//        //    {
//        //        HandleControllerException(exception);
//        //    }
//        //    return View();
//        //}

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(Commission model)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    model.ModifiedBy = User.Identity.GetUserId();
//                    model.ModifiedDate = DateTime.Now;

//                    _commissionService.Update(model);
//                    _commissionService.Save();
//                    return RedirectToAction("Index");
//                }

//                return View(model);
//            }
//            catch (Exception exception)
//            {
//                HandleControllerException(exception);
//            }
//            return View();
//        }

//        // GET: /CCustomerMaster/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            return View(_commissionService.GetById(id));
//        }

//        // POST: /CCustomerMaster/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            try
//            {
//                var commission = _commissionService.GetById(id);
//                commission.DeletedBy = User.Identity.GetUserId();
//                commission.Status = StatusConstants.Deleted;
//                commission.DeletedDate = DateTime.Now;

//                _commissionService.Delete(commission);
//                _commissionService.Save();
//                return RedirectToAction("Index");
//            }
//            catch (Exception exception)
//            {
//                HandleControllerException(exception);
//            }
//            return View();
//        }
//        #endregion

//    }
//}
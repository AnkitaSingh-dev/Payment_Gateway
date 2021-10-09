using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Corno.Raychem.CustomerPortal.Controllers;
using Microsoft.AspNet.Identity;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    public class MposController : BaseController
    {
        #region -- Constructors --
        public MposController(IMposService mposService)
        {
            _mposService = mposService;
        }
        #endregion

        #region -- Data Members --
        private readonly IMposService _mposService;
        #endregion

        #region -- Actions --
        // GET: /Mpos/
        public ActionResult Index(int? page)
        {
            var models = _mposService.Get().ToList();
            return View(models);
        }
        // GET: /Mpos/Details/5
        public ActionResult Details(int? id)
        {
            var model = new Mpos();

            try
            {
                if (id == null)
                    throw new Exception("This is an invalid User ID.");

                model = _mposService.GetById(id);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View(model);
        }

        // GET: /Mpos/Create
        public ActionResult Create()
        {
            return View(new Mpos());
        }

        // POST: /Mpos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mpos model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Check whether Name Already Exists
                var nameExisting = _mposService.Get(t => t.Mid == model.Mid).FirstOrDefault();
                if (null != nameExisting)
                    throw new Exception("An account for this Name already exists!");

                _mposService.Add(model);
                _mposService.Save();

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        // GET: /Mpos/Edit/5
        public ActionResult Edit(int? id)
        {
            var model = new Mpos();
            try
            {
                if (id == null)
                    throw new DbEntityValidationException("This is an invalid User ID.");

                model = _mposService.GetById(id);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        // POST: /Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mpos model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.ModifiedBy = User.Identity.GetUserId();
                model.ModifiedDate = DateTime.Now;

                _mposService.Update(model);
                _mposService.Save();

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        // GET: /Country/Delete/5
        public ActionResult Delete(int? id)
        {
            var model = new Mpos();
            try
            {
                if (id == null)
                    throw new Exception("This is an invalid User ID.");

                model = _mposService.GetById(id);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(model);
        }

        // POST: /Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var model = _mposService.GetById(id);

                model.DeletedBy = User.Identity.GetUserId();
                model.DeletedDate = DateTime.Now;
                model.Status = StatusConstants.Deleted;

                _mposService.Update(model);
                _mposService.Save();
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}

using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Areas.Admin.Services;
using Corno.Raychem.CustomerPortal.Controllers;
using System;
using System.Net;
using System.Web.Mvc;
using Corno.Services.Bootstrapper;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Controllers
{
    [Authorize]
    public class AspNetRoleController : BaseController
    {
        private readonly IAspNetRoleService _aspnetroleService;

        public AspNetRoleController(IAspNetRoleService aspnetroleService)
        {
            _aspnetroleService = aspnetroleService;
        }

        // GET: /AspNetRole/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(_aspnetroleService.Get());
        }

        // GET: /AspNetRole/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new AspNetRole());
        }

        // POST: /AspNetRole/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AspNetRole aspNetRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var identityManager = (IdentityManager) Bootstrapper.GetService(typeof(IdentityManager));
                    identityManager.CreateRole(aspNetRole.Name);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(aspNetRole);
        }

        // GET: /AspNetRole/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var aspnetrole = _aspnetroleService.GetById(id);
            if (aspnetrole == null)
                return HttpNotFound();

            return View(aspnetrole);
        }

        // POST: /AspNetRole/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AspNetRole aspNetRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //var identityManager = (IdentityManager)Bootstrapper.GetService(typeof(IdentityManager));
                    //identityManager.EditRole(aspNetRole);

                    _aspnetroleService.Update(aspNetRole);
                    _aspnetroleService.Save();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(aspNetRole);
        }

        // GET: /AspNetRole/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var aspnetrole = _aspnetroleService.GetById(id);
            if (aspnetrole == null)
            {
                return HttpNotFound();
            }
            return View(aspnetrole);
        }

        // POST: /AspNetRole/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(AspNetRole aspNetRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var identityManager = (IdentityManager) Bootstrapper.GetService(typeof(IdentityManager));
                    identityManager.DeleteRole(aspNetRole);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(aspNetRole);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _aspnetroleService.Dispose(disposing);
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
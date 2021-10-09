using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Corno.Globals.Constants;
using Corno.Raychem.CustomerPortal.Areas.Admin.Models;
using Corno.Raychem.CustomerPortal.Controllers;
using Microsoft.AspNet.Identity;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Models;
using Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces;
using Microsoft.Ajax.Utilities;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Controllers
{
    [Authorize]
    public class WalletDashboardController : BaseController
    {
        #region -- Constructors --
        public WalletDashboardController(IWalletTransactionService walletTransactionService, IdentityManager identityManager)
        {
            _walletTransactionService = walletTransactionService;

            _identityManager = identityManager;
        }
        #endregion

        #region -- Data Members --
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IdentityManager _identityManager;
        #endregion

        public ActionResult Index()
        {
            try
            {
                var user = _identityManager.GetUser(User.Identity.GetUserId());
                var viewModel = new WalletDashboardViewModel
                {
                    FullName = user.FirstName + " " + user.LastName,
                    WalletBlance = user.Wallet,

                    Services = _walletTransactionService.Get().Select(w => new {w.Service}).DistinctBy(w => w.Service).ToList(),
                    Operators = _walletTransactionService.Get().Select(w => new { w.Operator }).DistinctBy(w => w.Operator).ToList(),
                    PaymentModes = _walletTransactionService.Get().Select(w => new { w.PaymentMode }).DistinctBy(w => w.PaymentMode).ToList(),
                    Users = _walletTransactionService.Get().Select(w => new { w.UserName }).DistinctBy(w => w.UserName).ToList()
                };
                return View(viewModel);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }

            return View(new WalletDashboardViewModel());
        }

        public IEnumerable<double[]> GetTransactionTotals(Expression<Func<WalletTransaction, bool>> filter = null)
        {
            if (null == filter) return null;

            var transactions = _walletTransactionService.Get().AsQueryable().Where(filter)
                        .GroupBy(o => o.TransactionDate.Value.Month)
                        .OrderBy(o => o.Key)
                        .Select(o => new[]
                        {
                            o.Key,
                            o.Sum(t => t.Amount)
                        }).ToList();

            return transactions;
        }

        public ActionResult GetServiceDeta(string service, string transactionStatus)
        {
            try
            {
                Expression<Func<WalletTransaction, bool>> filter = o => o.Id > 0;

                if (false == User.IsInRole(RoleNames.Admin) && false == User.IsInRole(RoleNames.Viewer))
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.UserName == User.Identity.Name;
                }
                if (service == "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.Status == transactionStatus;
                }
                if (service != "All" && transactionStatus == "All")
                {
                    var prefix = filter.Compile();
                    filter = p => prefix(p) && p.Service == service;
                }
                if (service != "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.Service == service && o.Status == transactionStatus;
                }

                return Json(GetTransactionTotals(filter), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return null;
        }

        public ActionResult GetOperatorDeta(string operatorName, string transactionStatus)
        {
            try
            {
                Expression<Func<WalletTransaction, bool>> filter = o => o.Id > 0;

                if (false == User.IsInRole(RoleNames.Admin) && false == User.IsInRole(RoleNames.Viewer))
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.UserName == User.Identity.Name;
                }
                if (operatorName == "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.Status == transactionStatus;
                }
                if (operatorName != "All" && transactionStatus == "All")
                {
                    var prefix = filter.Compile();
                    filter = p => prefix(p) && p.Operator == operatorName;
                }
                if (operatorName != "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.Operator == operatorName && o.Status == transactionStatus;
                }

                return Json(GetTransactionTotals(filter), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return null;
        }

        public ActionResult GetPaymentModeDeta(string paymentMode, string transactionStatus)
        {
            try
            {
                Expression<Func<WalletTransaction, bool>> filter = o => o.Id > 0;

                if (false == User.IsInRole(RoleNames.Admin) && false == User.IsInRole(RoleNames.Viewer))
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.UserName == User.Identity.Name;
                }
                if (paymentMode == "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.Status == transactionStatus;
                }
                if (paymentMode != "All" && transactionStatus == "All")
                {
                    var prefix = filter.Compile();
                    filter = p => prefix(p) && p.PaymentMode == paymentMode;
                }
                if (paymentMode != "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.PaymentMode == paymentMode && o.Status == transactionStatus;
                }

                return Json(GetTransactionTotals(filter), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return null;
        }

        public ActionResult GetUserData(string userName, string transactionStatus)
        {
            try
            {
                Expression<Func<WalletTransaction, bool>> filter = o => o.Id > 0;

                if (false == User.IsInRole(RoleNames.Admin) && false == User.IsInRole(RoleNames.Viewer))
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.UserName == User.Identity.Name;
                }
                if (userName == "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.Status == transactionStatus;
                }
                if (userName != "All" && transactionStatus == "All")
                {
                    var prefix = filter.Compile();
                    filter = p => prefix(p) && p.UserName == userName;
                }
                if (userName != "All" && transactionStatus != "All")
                {
                    var prefix = filter.Compile();
                    filter = o => prefix(o) && o.UserName == userName && o.Status == transactionStatus;
                }

                return Json(GetTransactionTotals(filter), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return null;
        }
    }
}
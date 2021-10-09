using System.Web.Mvc;

//using BookERP.DataSets;
//using BookERP.Reports;

namespace Corno.Raychem.CustomerPortal.Areas.Reports.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Reports/Report/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string reportName, string description)
        {
            ViewData["ReportName"] = reportName;
            ViewData["ReportDescription"] = description;

            HttpContext.Session["ReportName"] = reportName;

            return View();
        }

        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                //ViewData["ReportName"] = reportName;
                View(actionName).ExecuteResult(ControllerContext);
            }
            catch
            {
                Response.Redirect("Page Not Found");
            }
        }
    }
}
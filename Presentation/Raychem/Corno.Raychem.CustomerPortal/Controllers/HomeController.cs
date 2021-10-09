using System.Web.Mvc;

namespace Corno.Raychem.CustomerPortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "The complete business portal";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }

        public ActionResult JsonValues()
        {
            return Json(
                new[] { new[] { 0.0, 1.0 }, new[] { 1.0, 0.5 }, new[] { 2.0, 2.0 } },
                JsonRequestBehavior.AllowGet);
        }
    }
}
using Corno.Raychem.CustomerPortal.AreaLib;
using System.Web.Mvc;
using System.Web.Routing;

namespace Corno.Raychem.CustomerPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapAreas("{controller}/{action}/{id}",
                "Areas",
                new[] { "Transactions", "Admin", "Account", "Masters", "Wallet", "Reports" });

            routes.MapRootArea("{controller}/{action}/{id}",
                "Areas",
                new { controller = "Home", action = "Index", id = "" });
        }
    }
}
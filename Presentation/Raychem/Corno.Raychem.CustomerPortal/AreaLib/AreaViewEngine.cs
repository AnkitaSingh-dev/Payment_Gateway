using System.Web.Mvc;

namespace Corno.Raychem.CustomerPortal.AreaLib
{
    public class AreaViewEngine : RazorViewEngine
    {
        public AreaViewEngine()
        {
            ViewLocationFormats = new[]
            {
                "~/{0}.aspx",
                "~/{0}.ascx",
                "~/{0}.cshtml",
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.aspx",
                "~/Views/Shared/{0}.ascx",
                "~/Views/Shared/{0}.cshtml"
            };

            MasterLocationFormats = new[]
            {
                "~/{0}.master",
                "~/Shared/{0}.master",
                "~/Views/{1}/{0}.master",
                "~/Views/Shared/{0}.master"
            };

            PartialViewLocationFormats = ViewLocationFormats;
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
            bool useCache)
        {
            ViewEngineResult areaResult = null;

            if (controllerContext.RouteData.Values.ContainsKey("area"))
            {
                var areaPartialName = FormatViewName(controllerContext, partialViewName);
                areaResult = base.FindPartialView(controllerContext, areaPartialName, useCache);
                if (areaResult != null && areaResult.View != null)
                {
                    return areaResult;
                }
                var sharedAreaPartialName = FormatSharedViewName(controllerContext, partialViewName);
                areaResult = base.FindPartialView(controllerContext, sharedAreaPartialName, useCache);
                if (areaResult != null && areaResult.View != null)
                {
                    return areaResult;
                }
            }

            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName,
            string masterName, bool useCache)
        {
            ViewEngineResult areaResult = null;

            if (controllerContext.RouteData.Values.ContainsKey("area"))
            {
                var areaViewName = FormatViewName(controllerContext, viewName);
                areaResult = base.FindView(controllerContext, areaViewName, masterName, useCache);
                if (areaResult != null && areaResult.View != null)
                {
                    return areaResult;
                }
                var sharedAreaViewName = FormatSharedViewName(controllerContext, viewName);
                areaResult = base.FindView(controllerContext, sharedAreaViewName, masterName, useCache);
                if (areaResult != null && areaResult.View != null)
                {
                    return areaResult;
                }
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        private static string FormatViewName(ControllerContext controllerContext, string viewName)
        {
            var controllerName = controllerContext.RouteData.GetRequiredString("controller");

            var area = controllerContext.RouteData.Values["area"].ToString();
            return "Areas/" + area + "/Views/" + controllerName + "/" + viewName;
        }

        private static string FormatSharedViewName(ControllerContext controllerContext, string viewName)
        {
            var area = controllerContext.RouteData.Values["area"].ToString();
            return "Areas/" + area + "/Views/Shared/" + viewName;
        }
    }
}
using Corno.Globals;
using Corno.Raychem.CustomerPortal.AreaLib;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Corno.Raychem.CustomerPortal.Helpers;

namespace Corno.Raychem.CustomerPortal
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new AreaViewEngine());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //BootstrapEditorTemplatesConfig.RegisterBundles();
            //Setup DI
            //GlobalVariables.Container = Bootstrapper.Initialise();
            BootstrapperLocal.BootStrapperInitialize();
            AutoMapperConfig.RegisterMappings();

            // Log4Net
            log4net.Config.XmlConfigurator.Configure();

            // Set Connection String
            GlobalVariables.ConnectionString =
                ConfigurationManager.ConnectionStrings["CornoContext"].ConnectionString;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // At this point we have information about the error
            var ctx = HttpContext.Current;
            var exception = ctx.Server.GetLastError();
            var errorInfo =
                "<br>Offending URL: " + ctx.Request.Url +
                "<br>Source: " + exception.Source +
                "<br>Message: " + exception.Message +
                "<br>Stack trace: " + exception.StackTrace;

            ctx.Response.Write(errorInfo);
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-Powered-By");
        }
    }
}
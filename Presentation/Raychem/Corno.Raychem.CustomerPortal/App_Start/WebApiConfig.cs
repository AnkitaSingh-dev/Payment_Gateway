using System.Web.Http;
using Corno.Raychem.CustomerPortal.Helpers;
using Microsoft.Practices.Unity.WebApi;
using Newtonsoft.Json.Serialization;
//using Telerik.Reporting.Services.WebApi;

namespace Corno.Raychem.CustomerPortal
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    "DefaultApi",
            //    "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional }
            //    );

            //config.Formatters.Remove(config.Formatters.JsonFormatter);

            //var serializer = new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() };
            //var formatter = new JsonMediaTypeFormatter { Indent = true, SerializerSettings = serializer };
            //config.Formatters.Add(formatter);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new LowercaseContractResolver();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                "SafexApi",
                "v2/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });
            //ReportsControllerConfiguration.RegisterRoutes(config);

            // Set Dependance Relolution
            BootstrapperLocal.BootStrapperInitialize();
            config.DependencyResolver = new UnityDependencyResolver(Globals.GlobalVariables.Container);
            
            // Initialize Web API Activator
            //UnityWebApiActivator.Start();
        }
    }

    // Custom class by Concept Systems
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
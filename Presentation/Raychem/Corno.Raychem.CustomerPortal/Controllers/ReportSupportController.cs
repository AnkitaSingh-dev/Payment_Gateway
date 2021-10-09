using System.Web;
using Telerik.Reporting.Cache.Interfaces;
using Telerik.Reporting.Services.Engine;
using Telerik.Reporting.Services.WebApi;

namespace Corno.Raychem.CustomerPortal.Controllers
{
    public class ReportsController : ReportsControllerBase
    {
        protected override IReportResolver CreateReportResolver()
        {
            var reportsPath = HttpContext.Current.Server.MapPath(@"~/Reports");
            return new ReportFileResolver(reportsPath)
              .AddFallbackResolver(new ReportTypeResolver());

            //var appPath = HttpContext.Current.Server.MapPath("~/");
            //return new ReportFileResolver(appPath)
            //.AddFallbackResolver(new ReportTypeResolver());
        }

        protected override ICache CreateCache()
        {
            return Telerik.Reporting.Services.Engine.CacheFactory.CreateFileCache();
        }
    }
}
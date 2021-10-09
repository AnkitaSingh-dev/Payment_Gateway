using System;
using System.Data.Entity.Validation;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace Corno.Raychem.CustomerPortal.Controllers
{
    [Authorize]
    [Compress]
    public class BaseController : Controller
    {
        #region -- Methods -- 
        public void HandleControllerException(Exception exception)
        {
            // Clear the ModelState
            ModelState.Clear();

            if (exception.GetType() == typeof(DbEntityValidationException))
            {
                var dbEx = exception as DbEntityValidationException;
                if (dbEx != null)
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
                ModelState.AddModelError(error.ErrorMessage, error.Exception);

            ModelState.AddModelError(string.Empty, exception.Message);

            if (exception.InnerException == null) return;

            ModelState.AddModelError(string.Empty, exception.InnerException.Message);
            if (exception.InnerException.InnerException != null)
                ModelState.AddModelError(string.Empty, exception.InnerException.InnerException.Message);
        }
        #endregion

        #region -- Events --
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var exception = filterContext.Exception;

            var result = View("~/Views/Error/Error.cshtml", new HandleErrorInfo(exception,
                                                                   filterContext.RouteData.Values["controller"].ToString(),
                                                                   filterContext.RouteData.Values["action"].ToString()));
            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
        }
        #endregion
    }

    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = filterContext.HttpContext.Response;

            if (encodingsAccepted.Contains("deflate"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encodingsAccepted.Contains("gzip"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}
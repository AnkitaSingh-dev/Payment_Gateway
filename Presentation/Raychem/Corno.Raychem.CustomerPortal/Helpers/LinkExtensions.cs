using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Corno.Raychem.CustomerPortal.Helpers
{
    public static class LinkExtensions
    {
        public static IHtmlString ActionLinkIfInRole(
            this HtmlHelper htmlHelper,
            string roles,
            string linkText,
            string action,
            object routValues
            )
        {
            if (!HttpContext.Current.User.IsInRole(roles))
            {
                return MvcHtmlString.Empty;
            }
            return htmlHelper.ActionLink(linkText, action, routValues);
        }

        public static bool IsInAnyRole(this IPrincipal principal, params string[] roles)
        {
            foreach (var role in roles)
            {
                if (principal.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
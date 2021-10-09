using System.Web.Mvc;

namespace Corno.Raychem.CustomerPortal.Helpers
{
    public static class HtmlExtensions
    {
        public static string SubmitLink(this HtmlHelper helper, string text, string formId)
        {
            return string.Format("<a class='k-button' onclick='$(\"#{1}\").submit();'>{1}</a>", text, formId);
        }
    }
}
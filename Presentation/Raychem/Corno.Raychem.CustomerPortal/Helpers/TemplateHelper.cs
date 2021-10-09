using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Corno.Raychem.CustomerPortal.Helpers
{
    public static class TemplateHelper
    {
        public static void RenderClientTemplate(this HtmlHelper helper, Type _type, string _partialViewName)
        {
            var model = Activator.CreateInstance(_type);
            helper.RenderPartial(_partialViewName, model);
        }
    }
}
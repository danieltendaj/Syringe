using System.Web.Mvc;

namespace Syringe.Web.Extensions.HtmlHelpers
{
    public static class NavLinks
    {
        public static MvcHtmlString Active(this HtmlHelper helper, string actionName, string controllerName)
        {
            string activeClass = "";
            var routeData = helper.ViewContext.RouteData.Values;

            if (routeData["action"].ToString() == actionName && routeData["controller"].ToString() == controllerName)
            {
                activeClass = "active";
            }
            return new MvcHtmlString(activeClass);
        }
    }
}
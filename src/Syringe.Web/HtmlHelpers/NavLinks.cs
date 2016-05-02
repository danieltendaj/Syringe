using System.Web.Mvc;

namespace Syringe.Web.HtmlHelpers
{
    public static class NavLinks
    {
        public static MvcHtmlString Active(this HtmlHelper helper, string action, string controller)
        {
            string activeClass = "";
            var routeData = helper.ViewContext.RouteData;
            if (routeData.Values["action"].ToString() == action && routeData.Values["controller"].ToString() == controller)
            {
                activeClass = "active";
            }
            return new MvcHtmlString(activeClass);
        }
    }
}
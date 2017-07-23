using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Syringe.Web.Extensions.HtmlHelpers
{
	public static class NavLinks
	{
		public static HtmlString Active(this IHtmlHelper<dynamic> helper, string actionName, string controllerName)
		{
			string activeClass = "";
			var routeData = helper.ViewContext.RouteData.Values;

			if (routeData["action"].ToString() == actionName && routeData["controller"].ToString() == controllerName)
			{
				activeClass = "active";
			}
			return new HtmlString(activeClass);
		}
	}
}
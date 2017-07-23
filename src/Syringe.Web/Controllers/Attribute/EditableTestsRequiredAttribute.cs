using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Syringe.Core.Configuration;

namespace Syringe.Web.Controllers.Attribute
{
	public class EditableTestsRequiredAttribute : ActionFilterAttribute, IActionFilter
	{
		public IConfiguration Configuration { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//if (Configuration.ReadonlyMode)
			//{
			//	throw new HttpException((int)HttpStatusCode.Forbidden, "Syringe is running in ReadOnly mode.");
			//}

			//base.OnActionExecuting(filterContext);
		}
	}
}
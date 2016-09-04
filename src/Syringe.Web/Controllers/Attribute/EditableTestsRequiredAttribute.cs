using System;
using System.Web.Mvc;
using Syringe.Core.Configuration;

namespace Syringe.Web.Controllers.Attribute
{
	public class EditableTestsRequiredAttribute : ActionFilterAttribute, IActionFilter
    {
        public IConfiguration Configuration { get; set; }

	    public override void OnActionExecuting(ActionExecutingContext filterContext)
	    {
            if (Configuration.ReadonlyMode)
            {
                throw new AccessViolationException();
            }

            base.OnActionExecuting(filterContext);
	    }
	}
}
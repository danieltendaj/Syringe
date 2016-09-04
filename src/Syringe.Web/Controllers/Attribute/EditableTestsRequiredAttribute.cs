using System.Reflection;
using System.Web.Mvc;
using Syringe.Core.Configuration;

namespace Syringe.Web.Controllers.Attribute
{
	public class EditableTestsRequiredAttribute : ActionMethodSelectorAttribute
    {
        public IConfiguration Configuration { get; set; }

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			if (Configuration.ReadonlyMode)
			{
				return false;
			}

			return true;
		}
	}
}
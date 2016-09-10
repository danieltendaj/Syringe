using System.Web.Mvc;
using Syringe.Core.Logging;

namespace Syringe.Web
{
	public class LogExceptionsAttribute : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			// TODO
		}
	}
}
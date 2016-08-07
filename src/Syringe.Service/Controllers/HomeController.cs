using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace Syringe.Service.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class HomeController : ApiController
	{
		[Route("~/")]
		[HttpGet]
		public RedirectResult Get()
		{
			return Redirect(Request.RequestUri + "swagger/ui/index");
		}
	}
}

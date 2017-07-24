using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace Syringe.Service.Controllers
{
	public class HomeController : ApiController
	{
		[Route("~/")]
		[HttpGet]
		public RedirectResult Get()
		{
			return Redirect(Request.RequestUri + "swagger/");
		}
	}
}
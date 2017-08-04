using Microsoft.AspNetCore.Mvc;

namespace Syringe.Service.Controllers
{
	public class HomeController : Controller
	{
		[Route("~/")]
		[HttpGet]
		public RedirectResult Get()
		{
			return Redirect("/swagger/");
		}
	}
}
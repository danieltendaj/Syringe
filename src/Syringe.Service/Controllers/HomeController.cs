using System.Web.Http;
using Microsoft.AspNetCore.Mvc;

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

using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Environment;
using Syringe.Service.Services;

namespace Syringe.Service.Controllers
{
	public class EnvironmentsController : ApiController, IEnvironmentsService
	{
		private readonly IEnvironmentProvider _provider;

		public EnvironmentsController(IEnvironmentProvider provider)
		{
			_provider = provider;
		}

		[Route("api/environments")]
        [HttpGet]
        public IEnumerable<Environment> Get()
        {
	        return _provider.GetAll();
        }
    }
}
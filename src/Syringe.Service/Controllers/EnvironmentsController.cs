using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Syringe.Core.Environment;
using Syringe.Core.Services;

namespace Syringe.Service.Controllers
{
	public class EnvironmentsController : Controller, IEnvironmentsService
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
using System;
using System.Web.Http;
using Syringe.Core.Configuration;
using Syringe.Core.MongoDB;
using Syringe.Core.Services;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Service.Api
{
	public class ConfigurationController : ApiController, IConfigurationService
	{
		private readonly IConfiguration _configuration;

		public ConfigurationController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[Route("api/configuration/")]
		[HttpGet]
		public IConfiguration GetConfiguration()
		{
			return _configuration;
		}

		[Route("api/WipeDatabase/")]
		[HttpGet]
		public string WipeDatabase()
		{
			try
			{
				var repository = new TestFileResultRepository(new MongoDbConfiguration(_configuration));
				repository.Wipe();

				return "done";
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}
	}
}

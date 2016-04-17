using System;
using System.Web.Http;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Repositories.XML;

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

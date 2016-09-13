using System.IO;
using System.Web.Http;
using Syringe.Core.Configuration;

namespace Syringe.Service.Controllers
{
	public class HealthCheckController : ApiController
	{
		private readonly IConfiguration _configuration;

		public HealthCheckController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[Route("api/healthcheck/CheckConfiguration")]
		[HttpGet]
		public string CheckConfiguration()
		{
			if (string.IsNullOrEmpty(_configuration.WebsiteUrl))
				return "The service WebsiteUrl key is empty - please enter the website url including port number in configuration.json, e.g. http://localhost:1980";

			if (string.IsNullOrEmpty(_configuration.TestFilesBaseDirectory))
				return "The service TestFilesBaseDirectory is empty - please enter the folder the test XML files are stored in configuration.json, e.g. D:\\syringe";

			if (!Directory.Exists(_configuration.TestFilesBaseDirectory))
				return string.Format("The service TestFilesBaseDirectory folder '{0}' does not exist", _configuration.TestFilesBaseDirectory);

			return "Everything is OK";
		}
	}
}
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Syringe.Core.Configuration;

namespace Syringe.Service.Controllers
{
	public class HealthCheckController : Controller
	{
		private readonly Settings _settings;

		public HealthCheckController(Settings settings)
		{
			_settings = settings;
		}

		[Route("api/healthcheck/CheckConfiguration")]
		[HttpGet]
		public string CheckConfiguration()
		{
			if (string.IsNullOrEmpty(_settings.WebsiteUrl))
				return "The service WebsiteUrl key is empty - please enter the website url including port number in configuration.json, e.g. http://localhost:1980";

			if (string.IsNullOrEmpty(_settings.TestFilesBaseDirectory))
				return "The service TestFilesBaseDirectory is empty - please enter the folder the test XML files are stored in configuration.json, e.g. D:\\syringe";

			if (!Directory.Exists(_settings.TestFilesBaseDirectory))
				return string.Format("The service TestFilesBaseDirectory folder '{0}' does not exist", _settings.TestFilesBaseDirectory);

			return "Everything is OK";
		}
	}
}
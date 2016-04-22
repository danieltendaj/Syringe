using System;
using System.IO;
using Newtonsoft.Json;

namespace Syringe.Web
{
	public class MvcConfiguration
	{
		private readonly Lazy<string> _lazySignalRUrl;
		private static MvcConfiguration _configuration;

		public string ServiceUrl { get; set; }
		public string SignalRUrl => _lazySignalRUrl.Value;

		private MvcConfiguration()
		{
			_lazySignalRUrl = new Lazy<string>(BuildSignalRUrl);
		}

		public static MvcConfiguration Load()
		{
			if (_configuration == null)
			{
				string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websiteconfig.json");
				string json = File.ReadAllText(configPath);
				MvcConfiguration configuration = JsonConvert.DeserializeObject<MvcConfiguration>(json);

				if (string.IsNullOrEmpty(configuration.ServiceUrl))
				{
					configuration.ServiceUrl = "http://localhost:1981";
				}

				_configuration = configuration;
			}

			return _configuration;
		}

		private string BuildSignalRUrl()
		{
			var builder = new UriBuilder(ServiceUrl);
			builder.Path = "signalr";
			return builder.ToString();
		}
	}
}
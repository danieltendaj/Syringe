using System;
using System.IO;
using Newtonsoft.Json;
using Syringe.Core.Exceptions;

namespace Syringe.Web
{
	public class MvcConfiguration
	{
		private readonly Lazy<string> _lazySignalRUrl;
		internal static MvcConfiguration Configuration;

		public string ServiceUrl { get; set; }
		public string SignalRUrl => _lazySignalRUrl.Value;

		internal MvcConfiguration()
		{
			_lazySignalRUrl = new Lazy<string>(BuildSignalRUrl);
			ServiceUrl = "http://localhost:1981";
		}

		public static MvcConfiguration Load()
		{
			if (Configuration == null)
			{
				string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websiteconfig.json");
				if (!File.Exists(configPath))
					throw new ConfigurationException("The website configuration file {0} could not be found", configPath);

				string json = File.ReadAllText(configPath);
				MvcConfiguration configuration = JsonConvert.DeserializeObject<MvcConfiguration>(json);

				if (string.IsNullOrEmpty(configuration.ServiceUrl))
					configuration.ServiceUrl = "http://localhost:1981";

				Configuration = configuration;
			}

			return Configuration;
		}

		private string BuildSignalRUrl()
		{
			var builder = new UriBuilder(ServiceUrl);
			builder.Path = "signalr";
			return builder.ToString();
		}
	}
}
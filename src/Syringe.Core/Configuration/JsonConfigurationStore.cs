using System;
using System.IO;
using Newtonsoft.Json;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Configuration
{
	public class JsonConfigurationStore : IConfigurationStore
	{
		private IConfiguration _configuration;
		private readonly string _configPath;

		public JsonConfigurationStore()
		{
			_configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.json");
		}

		public IConfiguration Load()
		{
			if (_configuration == null)
			{
				if (!File.Exists(_configPath))
					throw new ConfigurationException("The REST service configuration.json file does not exist: '{0}'", _configPath);

				string json = File.ReadAllText(_configPath);
				JsonConfiguration configuration = JsonConvert.DeserializeObject<JsonConfiguration>(json);

				if (!string.IsNullOrEmpty(configuration.TestFilesBaseDirectory))
				{
					if (configuration.TestFilesBaseDirectory.StartsWith(".."))
					{
						// Convert a relative path
						string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configuration.TestFilesBaseDirectory);
						configuration.TestFilesBaseDirectory = Path.GetFullPath(fullPath);
					}
					else
					{
						configuration.TestFilesBaseDirectory = Path.GetFullPath(configuration.TestFilesBaseDirectory);
					}
				}

				_configuration = configuration;
			}

			return _configuration;
		}
	}
}
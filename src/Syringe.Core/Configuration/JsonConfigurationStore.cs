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

		internal JsonConfigurationStore(string configPath)
		{
			_configPath = configPath;
		}

		public IConfiguration Load()
        {
            if (_configuration == null)
            {
                if (!File.Exists(_configPath))
                {
                    throw new ConfigurationException("The REST service configuration.json file does not exist: '{0}'", _configPath);
                }

                string json = File.ReadAllText(_configPath);
                JsonConfiguration configuration = JsonConvert.DeserializeObject<JsonConfiguration>(json);

                configuration.TestFilesBaseDirectory = ResolveRelativePath(configuration.TestFilesBaseDirectory);
                configuration.ScriptSnippetDirectory = ResolveRelativePath(configuration.ScriptSnippetDirectory);
                _configuration = configuration;
            }

            return _configuration;
        }

        private string ResolveRelativePath(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return directoryPath;

            if (directoryPath.StartsWith(".."))
            {
                // Convert to a relative path
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directoryPath);
                directoryPath = Path.GetFullPath(fullPath);
            }
            else
            {
                directoryPath = Path.GetFullPath(directoryPath);
            }

            return directoryPath;
        }
    }
}
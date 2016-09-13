using System;
using System.IO;
using Newtonsoft.Json;

namespace Syringe.Core.Configuration
{
    public class JsonConfigurationStore : IConfigurationStore
    {
        private readonly IConfigLocator _configLocator;
        private IConfiguration _configuration;

        //TODO: Get configs to use this class, and then update the setup scripts & appveyor to load into appData

        public JsonConfigurationStore(IConfigLocator configLocator)
        {
            _configLocator = configLocator;
        }

        public IConfiguration Load()
        {
            if (_configuration == null)
            {
                string configPath = _configLocator.ResolveConfigFile("configuration.json");
                
                string json = File.ReadAllText(configPath);
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
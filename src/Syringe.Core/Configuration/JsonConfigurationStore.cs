using System;
using System.IO;
using Newtonsoft.Json;

namespace Syringe.Core.Configuration
{
    public class JsonConfigurationStore : IConfigurationStore
    {
        private IConfiguration _configuration;
        private readonly string[] _configurationDirectories;

        public JsonConfigurationStore()
        {
            _configurationDirectories = new[]
            {
                AppDomain.CurrentDomain.BaseDirectory,
                GetAppDataFolder()
            };
        }

        internal JsonConfigurationStore(params string[] configPathDirecties)
        {
            _configurationDirectories = configPathDirecties;
        }

        public IConfiguration Load()
        {
            if (_configuration == null)
            {
                string configPath = ResolveConfigFile("configuration.json");
                
                string json = File.ReadAllText(configPath);
                JsonConfiguration configuration = JsonConvert.DeserializeObject<JsonConfiguration>(json);

                configuration.TestFilesBaseDirectory = ResolveRelativePath(configuration.TestFilesBaseDirectory);
                configuration.ScriptSnippetDirectory = ResolveRelativePath(configuration.ScriptSnippetDirectory);
                _configuration = configuration;
            }

            return _configuration;
        }

        /// <summary>
        /// This will look in defined places on the computer for config files.
        /// These might live in the current directory or /AppData/Syringe/
        /// </summary>
        public string ResolveConfigFile(string fileName)
        {
            foreach (string directory in _configurationDirectories)
            {
                string configToTest = Path.Combine(directory, fileName);
                if (File.Exists(configToTest))
                {
                    return configToTest;
                }
            }

            string errorMessage = $"Unable to find config file in: {string.Join(" | or | ", _configurationDirectories)}";
            throw new FileNotFoundException(errorMessage, fileName);
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

        private static string GetAppDataFolder()
        {
            return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "Syringe");
        }
    }
}
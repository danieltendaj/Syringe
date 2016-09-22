using System.IO;
using Microsoft.Extensions.Options;

namespace Syringe.Core.Configuration
{
    public class JsonConfigurationStore : IConfigurationStore
    {
        private readonly IOptions<JsonConfiguration> _options;
        private JsonConfiguration _configuration;
        
        public JsonConfigurationStore(IOptions<JsonConfiguration> options)
        {
            _options = options;
        }

        public IConfiguration Load()
        {
            if (_configuration == null)
            {
                _configuration = _options.Value;
                _configuration.TestFilesBaseDirectory = ResolveRelativePath(_configuration.TestFilesBaseDirectory);
                _configuration.ScriptSnippetDirectory = ResolveRelativePath(_configuration.ScriptSnippetDirectory);
            }

            return _configuration;
        }

        private string ResolveRelativePath(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return directoryPath;
            }

            if (directoryPath.StartsWith(".."))
            {
                // Convert to a relative path
                string fullPath = Path.Combine(System.AppContext.BaseDirectory, directoryPath);
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
using System.IO;
using Newtonsoft.Json;
using Syringe.Core.Configuration;

namespace Syringe.Web.Configuration
{
    public class MvcConfigurationProvider : IMvcConfigurationProvider
    {
        private readonly IConfigLocator _configLocator;
        private MvcConfiguration _configuration;

        public MvcConfigurationProvider(IConfigLocator configLocator)
        {
            _configLocator = configLocator;
        }

        public MvcConfiguration Load()
        {
            if (_configuration == null)
            {
                string configPath = _configLocator.ResolveConfigFile("websiteconfig.json");
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _configuration = JsonConvert.DeserializeObject<MvcConfiguration>(json);
                }
                else
                {
                    _configuration = new MvcConfiguration();
                }

                if (string.IsNullOrEmpty(_configuration.ServiceUrl))
                {
                    _configuration.ServiceUrl = "http://localhost:1981";
                }
            }

            return _configuration;
        }
    }
}
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
                string json = File.ReadAllText(configPath);
                _configuration = JsonConvert.DeserializeObject<MvcConfiguration>(json);
            }

            return _configuration;
        }
    }
}
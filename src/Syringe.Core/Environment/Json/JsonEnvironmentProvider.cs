using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Syringe.Core.Configuration;

namespace Syringe.Core.Environment.Json
{
    public class JsonEnvironmentProvider : IEnvironmentProvider
    {
        private readonly IConfigurationRoot _configurationRoot;
        //private readonly IConfigLocator _configLocator;
        private List<Environment> _environments;

        //public JsonEnvironmentProvider(IConfigLocator configLocator)
        //{
        //    _configLocator = configLocator;
        //}

        public JsonEnvironmentProvider(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public IEnumerable<Environment> GetAll()
        {
            if (_environments == null)
            {
                //string configPath = _configLocator.ResolveConfigFile("environments.json");
                //string json = File.ReadAllText(configPath);
                //List<Environment> environments = JsonConvert.DeserializeObject<List<Environment>>(json);

                var environments = _configurationRoot.GetValue<List<Environment>>("environments");
                _environments = environments.OrderBy(x => x.Order).ToList();
            }

            return _environments;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Syringe.Core.Configuration;

namespace Syringe.Core.Environment.Json
{
    public class JsonEnvironmentProvider : IEnvironmentProvider
    {
        private readonly IConfigLocator _configLocator;
        private List<Environment> _environments;

        public JsonEnvironmentProvider(IConfigLocator configLocator)
        {
            _configLocator = configLocator;
        }

        public IEnumerable<Environment> GetAll()
        {
            if (_environments == null)
            {
                string configPath = _configLocator.ResolveConfigFile("environments.json");
                string json = File.ReadAllText(configPath);
                List<Environment> environments = JsonConvert.DeserializeObject<List<Environment>>(json);

                _environments = environments.OrderBy(x => x.Order).ToList();
            }

            return _environments;
        }
    }
}
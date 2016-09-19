using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Syringe.Core.Configuration;

namespace Syringe.Core.Tests.Variables.SharedVariables
{
    public class SharedVariablesProvider : ISharedVariablesProvider
    {
        private readonly IConfigurationRoot _configurationRoot;
        //private readonly IConfigLocator _configLocator;
        private Variable[] _sharedVariables;

        //public SharedVariablesProvider(IConfigLocator configLocator)
        //{
        //    _configLocator = configLocator;
        //}

        public SharedVariablesProvider(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public IEnumerable<IVariable> ListSharedVariables()
        {
            if (_sharedVariables == null)
            {
                try
                {
                    //string configPath = _configLocator.ResolveConfigFile("shared-variables.json");
                    //string json = File.ReadAllText(configPath);
                    //List<SharedVariable> variables = JsonConvert.DeserializeObject<List<SharedVariable>>(json);

                    var variables = _configurationRoot.GetValue<List<SharedVariable>>("sharedVariables");
                    _sharedVariables = variables.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToArray();
                }
                catch (FileNotFoundException)
                {
                    _sharedVariables = new Variable[0];
                }
            }

            return _sharedVariables;
        }

        private class SharedVariable
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Environment { get; set; }
        }
    }
}
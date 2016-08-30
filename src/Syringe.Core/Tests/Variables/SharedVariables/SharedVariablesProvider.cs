using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Syringe.Core.Tests.Variables.SharedVariables
{
    public class SharedVariablesProvider : ISharedVariablesProvider
    {
        private readonly string _configPath;
        private Variable[] _sharedVariables;

        public SharedVariablesProvider()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shared-variables.json");
        }

        internal SharedVariablesProvider(string configPath)
        {
            _configPath = configPath;
        }

        public IEnumerable<IVariable> ListSharedVariables()
        {
            if (_sharedVariables == null)
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    List<SharedVariable> variables = JsonConvert.DeserializeObject<List<SharedVariable>>(json);
                    _sharedVariables = variables.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToArray();
                }
                else
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
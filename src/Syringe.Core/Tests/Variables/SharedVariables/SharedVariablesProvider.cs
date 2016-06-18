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

        public SharedVariablesProvider()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shared-variables.json");
        }

        internal SharedVariablesProvider(string configPath)
        {
            _configPath = configPath;
        }

        public IEnumerable<Variable> ListSharedVariables()
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                List<SharedVariable> variables = JsonConvert.DeserializeObject<List<SharedVariable>>(json);

                return variables.Select(x => new Variable(x.Name, x.Value, x.Environment));
            }

            return new Variable[0];
        }

        private class SharedVariable
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Environment { get; set; }
        }
    }
}
using System.Collections.Generic;

namespace Syringe.Core.Tests.Variables.SharedVariables
{
    public class SharedVariables : List<SharedVariables.SharedVariable>
    {
        public class SharedVariable
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Environment { get; set; }
        }
    }
}
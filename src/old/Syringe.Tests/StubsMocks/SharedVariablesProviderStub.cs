using System.Collections.Generic;
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Tests.StubsMocks
{
    public class SharedVariablesProviderStub : ISharedVariablesProvider
    {
        public IVariable[] ListSharedVariables_Value = new IVariable[0];
        public IEnumerable<IVariable> ListSharedVariables()
        {
            return ListSharedVariables_Value;
        }
    }
}
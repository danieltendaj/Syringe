using System.Collections;
using System.Collections.Generic;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.StubsMocks
{
    public class VariableContainerStub : IVariableContainer
    {
        public List<IVariable> Variables = new List<IVariable>(); 

        public IEnumerator<IVariable> GetEnumerator()
        {
            return Variables.GetEnumerator();
        }

        public void Add(IVariable variable)
        {
            Variables.Add(variable);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.StubsMocks
{
    public class VariableContainerStub : IVariableContainer
    {
        public List<Variable> Variables = new List<Variable>(); 

        public IEnumerator<Variable> GetEnumerator()
        {
            return Variables.GetEnumerator();
        }

        public void Add(Variable variable)
        {
            Variables.Add(variable);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
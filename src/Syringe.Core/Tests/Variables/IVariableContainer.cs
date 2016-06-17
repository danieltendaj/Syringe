using System.Collections.Generic;

namespace Syringe.Core.Tests.Variables
{
    public interface IVariableContainer : IEnumerable<Variable>
    {
        void Add(Variable variable);
    }
}
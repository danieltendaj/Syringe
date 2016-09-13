using System.Collections.Generic;

namespace Syringe.Core.Tests.Variables
{
    public interface IVariableContainer : IEnumerable<IVariable>
    {
        void Add(IVariable variable);
    }
}
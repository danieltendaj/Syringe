using System.Collections.Generic;

namespace Syringe.Core.Tests.Variables.SharedVariables
{
    public interface ISharedVariablesProvider
    {
        IEnumerable<Variable> ListSharedVariables();
    }
}
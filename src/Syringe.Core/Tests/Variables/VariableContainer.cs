using System.Collections;
using System.Collections.Generic;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Core.Tests.Variables
{
    public class VariableContainer : IVariableContainer
    {
        private readonly IReservedVariableProvider _reservedVariableProvider;
        private readonly ISharedVariablesProvider _sharedVariablesProvider;
        private readonly List<Variable> _variables = new List<Variable>();
        private IReservedVariable[] _reservedVariables;

        public VariableContainer(IReservedVariableProvider reservedVariableProvider, ISharedVariablesProvider sharedVariablesProvider)
        {
            _reservedVariableProvider = reservedVariableProvider;
            _sharedVariablesProvider = sharedVariablesProvider;
        }

        public IEnumerator<Variable> GetEnumerator()
        {
            _reservedVariables = _reservedVariableProvider.ListAvailableVariables();

            foreach (var reservedVariable in _reservedVariables)
            {
                yield return reservedVariable.CreateVariable();
            }

            foreach (var reservedVariable in _sharedVariablesProvider.ListSharedVariables())
            {
                yield return reservedVariable;
            }

            foreach (var variable in _variables)
            {
                yield return variable;
            }
        }

        public void Add(Variable variable)
        {
            _variables.Add(variable);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
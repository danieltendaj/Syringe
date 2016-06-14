using System.Collections;
using System.Collections.Generic;
using Syringe.Core.Tests.Variables.ReservedVariables;

namespace Syringe.Core.Tests.Variables
{
    public interface IVariableContainer : IEnumerable<Variable>
    {
        void Add(Variable variable);
    }

    public class VariableContainer : IVariableContainer
    {
        private readonly IReservedVariableProvider _reservedVariableProvider;
        private readonly List<Variable> _variables = new List<Variable>();
        private IReservedVariable[] _reservedVariables;

        public VariableContainer(IReservedVariableProvider reservedVariableProvider)
        {
            _reservedVariableProvider = reservedVariableProvider;
        }

        //TODO: Add reserved variables

        public IEnumerator<Variable> GetEnumerator()
        {
            _reservedVariables = _reservedVariableProvider.ListAvailableVariables();

            foreach (var reservedVariable in _reservedVariables)
            {
                yield return reservedVariable.CreateVariable();
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
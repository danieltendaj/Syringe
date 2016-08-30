using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Core.Tests.Variables
{
    public class VariableContainer : IVariableContainer
    {
        private readonly string _environment;
        private readonly IReservedVariableProvider _reservedVariableProvider;
        private readonly ISharedVariablesProvider _sharedVariablesProvider;
        private readonly List<IVariable> _variables = new List<IVariable>();

        public VariableContainer(string environment, IReservedVariableProvider reservedVariableProvider, ISharedVariablesProvider sharedVariablesProvider)
        {
            _environment = environment;
            _reservedVariableProvider = reservedVariableProvider;
            _sharedVariablesProvider = sharedVariablesProvider;
        }

        public IEnumerator<IVariable> GetEnumerator()
        {
            return
                _variables
                .Concat(GetSharedVariable())
                .OrderBy(x => x.Environment?.Name)
                .GroupBy(x => x.Name)
                .SelectMany(x => x)
                .Concat(_reservedVariableProvider.ListAvailableVariables().Select(x => x.CreateVariable()))
                .GetEnumerator();
        }

        private IEnumerable<IVariable> _sharedVariables;
        private IEnumerable<IVariable> GetSharedVariable()
        {
            return _sharedVariables ?? (_sharedVariables = _sharedVariablesProvider.ListSharedVariables().Where(x => x.MatchesEnvironment(_environment)));
        }

        public void Add(IVariable variable)
        {
            if (variable.MatchesEnvironment(_environment))
            {
                _variables.Add(variable);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
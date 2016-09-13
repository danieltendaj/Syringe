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
                .OrderBy(x => x.Name)
                .ThenByDescending(x => x.Environment?.Name)
                .Concat(GetReservedVariables())
                .GetEnumerator();
        }

        private IEnumerable<IVariable> _sharedVariables;
        private IEnumerable<IVariable> GetSharedVariable()
        {
            return _sharedVariables ?? (_sharedVariables = _sharedVariablesProvider.ListSharedVariables().Where(x => x.MatchesEnvironment(_environment)));
        }

        private IEnumerable<IVariable> _reservedVariables;
        private IEnumerable<IVariable> GetReservedVariables()
        {
            return _reservedVariables ?? (_reservedVariables = _reservedVariableProvider.ListAvailableVariables().Select(x => x.CreateVariable()));
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
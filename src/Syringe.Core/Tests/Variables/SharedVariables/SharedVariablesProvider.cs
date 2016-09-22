using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Syringe.Core.Tests.Variables.SharedVariables
{
    public class SharedVariablesProvider : ISharedVariablesProvider
    {
        private readonly IOptions<SharedVariables> _options;
        private Variable[] _sharedVariables;

        public SharedVariablesProvider(IOptions<SharedVariables> options)
        {
            _options = options;
        }

        public IEnumerable<IVariable> ListSharedVariables()
        {
            if (_sharedVariables == null)
            {
                if (_options.Value != null && _options.Value.Any())
                {
                    _sharedVariables = _options.Value.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToArray();
                }
                else
                {
                    _sharedVariables = new Variable[0];
                }
            }

            return _sharedVariables;
        }
    }
}
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Core.Runner
{
    public class CapturedVariableProviderFactory : ICapturedVariableProviderFactory
    {
        private readonly IVariableEncryptor _encryptor;
        private readonly ISharedVariablesProvider _sharedVariablesProvider;

        public CapturedVariableProviderFactory(IVariableEncryptor encryptor, ISharedVariablesProvider sharedVariablesProvider)
        {
            _encryptor = encryptor;
            _sharedVariablesProvider = sharedVariablesProvider;
        }

        public ICapturedVariableProvider Create(string environment)
        {
            var container = new VariableContainer(environment, new ReservedVariableProvider(environment), _sharedVariablesProvider);
            return new CapturedVariableProvider(container, environment, _encryptor);
        }
    }
}
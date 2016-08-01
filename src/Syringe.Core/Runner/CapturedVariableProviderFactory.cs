using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Core.Runner
{
    public class CapturedVariableProviderFactory : ICapturedVariableProviderFactory
    {
	    private readonly IVariableEncryptor _encryptor;
	    public CapturedVariableProviderFactory(IVariableEncryptor encryptor)
	    {
		    _encryptor = encryptor;
	    }

	    public ICapturedVariableProvider Create(string environment)
        {
            var container = new VariableContainer(new ReservedVariableProvider(environment), new SharedVariablesProvider());
            return new CapturedVariableProvider(container, environment, _encryptor);
        }
    }
}
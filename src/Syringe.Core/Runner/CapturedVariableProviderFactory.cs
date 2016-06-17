using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.ReservedVariables;

namespace Syringe.Core.Runner
{
    public class CapturedVariableProviderFactory : ICapturedVariableProviderFactory
    {
        public ICapturedVariableProvider Create(string environment)
        {
            var container = new VariableContainer(new ReservedVariableProvider());
            return new CapturedVariableProvider(container, environment);
        }
    }
}
using Syringe.Core.Tests.Variables;

namespace Syringe.Core.Runner
{
    public class CapturedVariableProviderFactory : ICapturedVariableProviderFactory
    {
        private readonly IVariableContainer _variableContainer;

        public CapturedVariableProviderFactory(IVariableContainer variableContainer)
        {
            _variableContainer = variableContainer;
        }

        public ICapturedVariableProvider Create(string environment)
        {
            return new CapturedVariableProvider(_variableContainer, environment);
        }
    }
}
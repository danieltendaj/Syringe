using Syringe.Core.Tests.Variables.ReservedVariables;

namespace Syringe.Tests.StubsMocks
{
    public class ReservedVariableProviderStub : IReservedVariableProvider
    {
        public IReservedVariable[] ListAvailableVariables_Value = new IReservedVariable[0];
        public IReservedVariable[] ListAvailableVariables()
        {
            return ListAvailableVariables_Value;
        }
    }
}
using Syringe.Core.Tests.Variables;
using Syringe.Core.Tests.Variables.ReservedVariables;

namespace Syringe.Tests.StubsMocks
{
    public class ReservedVariableStub : IReservedVariable
    {
        public string Description { get; set; }
        public string Name { get; set; }

        public Variable CreateVariable_Value { get; set; }
        public Variable CreateVariable()
        {
            return CreateVariable_Value;
        }
    }
}
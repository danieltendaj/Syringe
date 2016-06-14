namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public class ReservedVariableProvider : IReservedVariableProvider
    {
        public IReservedVariable[] ListAvailableVariables()
        {
            return new IReservedVariable[]
            {
                new RandomNumberVariable()
            };
        }
    }
}
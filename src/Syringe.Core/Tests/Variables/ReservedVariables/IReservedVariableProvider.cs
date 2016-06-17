namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public interface IReservedVariableProvider
    {
        IReservedVariable[] ListAvailableVariables();
    }
}
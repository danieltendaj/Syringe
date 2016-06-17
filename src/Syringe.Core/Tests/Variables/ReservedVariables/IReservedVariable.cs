namespace Syringe.Core.Tests.Variables.ReservedVariables
{
    public interface IReservedVariable
    {
        string Description { get; }
        string Name { get; }

        Variable CreateVariable();
    }
}
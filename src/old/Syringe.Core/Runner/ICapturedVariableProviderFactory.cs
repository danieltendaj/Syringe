namespace Syringe.Core.Runner
{
    public interface ICapturedVariableProviderFactory
    {
        ICapturedVariableProvider Create(string environment);
    }
}
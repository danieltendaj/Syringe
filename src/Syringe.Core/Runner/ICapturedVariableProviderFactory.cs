namespace Syringe.Core.Runner
{
    public interface ICapturedVariableProviderFactory
    {
        CapturedVariableProvider Create(string environment);
    }
}
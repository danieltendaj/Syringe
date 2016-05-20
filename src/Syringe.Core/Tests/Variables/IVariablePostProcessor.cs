namespace Syringe.Core.Tests.Variables
{
    public interface IVariablePostProcessor
    {
        string Process(string value, VariablePostProcessorType postProcessorType);
    }
}
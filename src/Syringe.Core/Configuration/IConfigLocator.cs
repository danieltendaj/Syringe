namespace Syringe.Core.Configuration
{
    public interface IConfigLocator
    {
        string ResolveConfigFile(string fileName);
    }
}
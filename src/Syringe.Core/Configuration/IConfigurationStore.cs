namespace Syringe.Core.Configuration
{
	public interface IConfigurationStore
	{
	    IConfiguration Load();
	    string ResolveConfigFile(string fileName);

	}
}
using Syringe.Core.Configuration;

namespace Syringe.Core.Services
{
	public interface IConfigurationService
	{
		IConfiguration GetConfiguration();
	}
}
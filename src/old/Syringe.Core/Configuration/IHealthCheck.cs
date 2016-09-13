namespace Syringe.Core.Configuration
{
	public interface IHealthCheck
	{
		void CheckServiceConfiguration();
		void CheckServiceSwaggerIsRunning();
	}
}
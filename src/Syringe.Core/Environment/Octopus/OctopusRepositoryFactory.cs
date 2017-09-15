using Syringe.Core.Configuration;

namespace Syringe.Core.Environment.Octopus
{
	public class OctopusRepositoryFactory : IOctopusRepositoryFactory
	{
		private readonly Settings _settings;

		public OctopusRepositoryFactory(Settings settings)
		{
			_settings = settings;
		}

		public IOctopusRepository Create()
		{
			return new OctopusRepository();
			//return new OctopusRepository(new OctopusServerEndpoint(_config.OctopusConfiguration.OctopusUrl, _config.OctopusConfiguration.OctopusApiKey));
		}
	}
}
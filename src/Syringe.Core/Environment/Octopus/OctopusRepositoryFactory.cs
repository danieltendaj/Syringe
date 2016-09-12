using Syringe.Core.Configuration;

namespace Syringe.Core.Environment.Octopus
{
    public class OctopusRepositoryFactory : IOctopusRepositoryFactory
    {
        private readonly IConfiguration _config;

        public OctopusRepositoryFactory(IConfiguration config)
        {
            _config = config;
        }

        public IOctopusRepository Create()
        {
            return new OctopusRepository();
            //return new OctopusRepository(new OctopusServerEndpoint(_config.OctopusConfiguration.OctopusUrl, _config.OctopusConfiguration.OctopusApiKey));
        }
    }
}
namespace Syringe.Core.Configuration
{
    public class MongoDbConfiguration
    {
        public string ConnectionString { get; private set; }

		public MongoDbConfiguration(IConfiguration configuration)
		{
			ConnectionString = configuration.MongoDbConnectionString;
		}
    }
}
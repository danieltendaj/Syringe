using Syringe.Core.Configuration;

namespace Syringe.Core.MongoDB
{
	public class MongoDbConfiguration
	{
		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }

		public MongoDbConfiguration(IConfiguration configuration)
		{
			ConnectionString = "mongodb://localhost:27017";
			DatabaseName = "Syringe";

			if (!string.IsNullOrEmpty(configuration.MongoDbDatabaseName))
				DatabaseName = configuration.MongoDbDatabaseName;
		}
	}
}
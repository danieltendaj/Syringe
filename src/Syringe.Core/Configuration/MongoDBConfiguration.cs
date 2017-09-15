namespace Syringe.Core.Configuration
{
	public class MongoDbConfiguration
	{
		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }

		public MongoDbConfiguration(Settings settings)
		{
			ConnectionString = "mongodb://localhost:27017";
			DatabaseName = "Syringe";

			if (!string.IsNullOrEmpty(settings.MongoDbDatabaseName))
			{
				DatabaseName = settings.MongoDbDatabaseName;
			}
		}
	}
}
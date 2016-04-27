namespace Syringe.Core.Configuration
{
	public interface IConfiguration
	{
		string ServiceUrl { get; }
		string WebsiteUrl { get; }
		string TestFilesBaseDirectory { get; }
	    TestFileFormat TestFileFormat { get; }
	    string MongoDbDatabaseName { get; }
		OAuthConfiguration OAuthConfiguration { get; }
		GitConfiguration GitConfiguration { get; }
        OctopusConfiguration OctopusConfiguration { get; }
    }
}
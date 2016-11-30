using System;

namespace Syringe.Core.Configuration
{
	public interface IConfiguration
	{
		string ServiceUrl { get; }
		string WebsiteUrl { get; }
        int EngineVersion { get; }
		string TestFilesBaseDirectory { get; }
		OAuthConfiguration OAuthConfiguration { get; }
        OctopusConfiguration OctopusConfiguration { get; }
		bool ReadonlyMode { get; }
        string ScriptSnippetDirectory { get; }
		string EncryptionKey { get; }
	    string MongoDbConnectionString { get; }
        int DaysOfDataRetention { get; }
	    TimeSpan CleanupSchedule { get; }
	}
}
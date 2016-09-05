﻿namespace Syringe.Core.Configuration
{
	public interface IConfiguration
	{
		string ServiceUrl { get; }
		string WebsiteUrl { get; }
		string TestFilesBaseDirectory { get; }
		OAuthConfiguration OAuthConfiguration { get; }
        OctopusConfiguration OctopusConfiguration { get; }
		bool ReadonlyMode { get; }
        string ScriptSnippetDirectory { get; }
		string EncryptionKey { get; }
	    string MongoDbDatabaseName { get; }
        int DaysOfDataRetention { get; }

    }
}
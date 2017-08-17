using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace Syringe.Core.Configuration
{
	public class JsonConfiguration : IConfiguration
	{
		public Settings Settings { get; set; }
		public IEnumerable<Environment.Environment> Environments { get; set; }
	}

	public class Settings
	{
		public string ServiceUrl { get; set; }
		public string WebsiteUrl { get; set; }
		public string TestFilesBaseDirectory { get; set; }
		public OAuthConfiguration OAuthConfiguration { get; set; }
		public OctopusConfiguration OctopusConfiguration { get; set; }
		public bool ReadonlyMode { get; set; }
		public string ScriptSnippetDirectory { get; set; }
		public string EncryptionKey { get; set; }
		public string MongoDbDatabaseName { get; set; }
		public int DaysOfDataRetention { get; set; }
		public TimeSpan CleanupSchedule { get; set; }

		//public Settings()
		//{
		//	// Defaults
		//	WebsiteUrl = "http://localhost:1980";
		//	ServiceUrl = "http://*:1981";
		//	TestFilesBaseDirectory = @"C:\Syringe\";
		//	ReadonlyMode = false;
		//	ScriptSnippetDirectory = Path.Combine(TestFilesBaseDirectory, "ScriptSnippets");
		//	MongoDbDatabaseName = "Syringe";
		//	DaysOfDataRetention = 10;
		//	CleanupSchedule = TimeSpan.FromHours(1);
		//	OAuthConfiguration = new OAuthConfiguration();
		//	OctopusConfiguration = new OctopusConfiguration();
		//}
	}
}
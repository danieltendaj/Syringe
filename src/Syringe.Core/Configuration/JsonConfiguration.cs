using System;
using Newtonsoft.Json;
using System.IO;

namespace Syringe.Core.Configuration
{
	public class JsonConfiguration : IConfiguration
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ServiceUrl { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string WebsiteUrl { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string TestFilesBaseDirectory { get; set; }
        
		public OAuthConfiguration OAuthConfiguration { get; set; }

        public OctopusConfiguration OctopusConfiguration { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool ReadonlyMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ScriptSnippetDirectory { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string EncryptionKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MongoDbDatabaseName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int DaysOfDataRetention { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan CleanupSchedule { get; set; }

        public JsonConfiguration()
		{
			// Defaults
			WebsiteUrl = "http://localhost:1980";
			ServiceUrl = "http://*:1981";
			TestFilesBaseDirectory = @"C:\Syringe\";
			ReadonlyMode = false;
            ScriptSnippetDirectory = Path.Combine(TestFilesBaseDirectory, "ScriptSnippets");
            MongoDbDatabaseName = "Syringe";
            DaysOfDataRetention = 5;
            CleanupSchedule = TimeSpan.FromHours(1);

            OAuthConfiguration = new OAuthConfiguration();
			OctopusConfiguration = new OctopusConfiguration();
		}
	}
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public TestFileFormat TestFileFormat { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string MongoDbDatabaseName { get; set; }

		public OAuthConfiguration OAuthConfiguration { get; set; }
        public OctopusConfiguration OctopusConfiguration { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool ReadonlyMode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DataStoreType DataStore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ScriptSnippetDirectory { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string EncryptionKey { get; set; }

		public JsonConfiguration()
		{
			// Defaults
			WebsiteUrl = "http://localhost:1980";
			ServiceUrl = "http://*:1981";
			TestFilesBaseDirectory = @"C:\Syringe\";
            TestFileFormat = TestFileFormat.Json;
			MongoDbDatabaseName = "Syringe";
			ReadonlyMode = false;
            DataStore = DataStoreType.MongoDb;
            ScriptSnippetDirectory = Path.Combine(TestFilesBaseDirectory, "ScriptSnippets");

			OAuthConfiguration = new OAuthConfiguration();
			OctopusConfiguration = new OctopusConfiguration();
		}
	}
}

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

	    public bool ContainsOAuthCredentials()
        {
            return (!string.IsNullOrEmpty(OAuthConfiguration?.GoogleAuthClientId) && !string.IsNullOrEmpty(OAuthConfiguration?.GoogleAuthClientSecret))
                    || (!string.IsNullOrEmpty(OAuthConfiguration?.MicrosoftAuthClientId) && string.IsNullOrEmpty(OAuthConfiguration?.MicrosoftAuthClientSecret))
                    || (!string.IsNullOrEmpty(OAuthConfiguration?.GithubAuthClientId) && !string.IsNullOrEmpty(OAuthConfiguration?.GithubAuthClientSecret));
        }

	    public JsonConfiguration()
		{
			// Defaults
			WebsiteUrl = "http://localhost:1980";
			ServiceUrl = "http://*:1981";
			TestFilesBaseDirectory = @"C:\Syringe\";
			ReadonlyMode = false;
            ScriptSnippetDirectory = Path.Combine(TestFilesBaseDirectory, "ScriptSnippets");
            MongoDbDatabaseName = "Syringe";

            OAuthConfiguration = new OAuthConfiguration();
			OctopusConfiguration = new OctopusConfiguration();
		}
	}
}

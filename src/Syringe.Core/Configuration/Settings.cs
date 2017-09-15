using System;

namespace Syringe.Core.Configuration
{
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
	}
}
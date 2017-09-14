using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Configuration
{
	public class JsonConfigurationStore : IConfigurationStore
	{
		private JsonConfiguration _jsonConfiguration;

		public IConfiguration Load()
		{
			if (_jsonConfiguration == null)
			{
				var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

				var configRoot = builder.Build();

				// The keys in appsettings.json are case sensitive
				_jsonConfiguration = configRoot.Get<JsonConfiguration>();
				EnsureSettings();
				EnsureEnvironments();

				_jsonConfiguration.Settings.TestFilesBaseDirectory = ResolveRelativePath(_jsonConfiguration.Settings.TestFilesBaseDirectory);
				_jsonConfiguration.Settings.ScriptSnippetDirectory = ResolveRelativePath(_jsonConfiguration.Settings.ScriptSnippetDirectory);
			}

			return _jsonConfiguration;
		}

		private void EnsureSettings()
		{
			// TODO: Test coverage
			if (_jsonConfiguration.Settings == null)
				throw new ConfigurationException("No Settings section was found in the appsettings.json configuration file.");

			if (_jsonConfiguration.Environments == null)
				throw new ConfigurationException("No Environments section was found in the appsettings.json configuration file.");

			if (string.IsNullOrWhiteSpace(_jsonConfiguration.Settings.MongoDbDatabaseName))
				throw new ConfigurationException("The database connection string was missing from the appsettings.json configuration file.");

			if (_jsonConfiguration.Settings.DaysOfDataRetention < 1)
				_jsonConfiguration.Settings.DaysOfDataRetention = 10;

			if (_jsonConfiguration.Settings.CleanupSchedule == TimeSpan.Zero)
				_jsonConfiguration.Settings.CleanupSchedule = TimeSpan.FromHours(1);

			if (string.IsNullOrWhiteSpace(_jsonConfiguration.Settings.TestFilesBaseDirectory))
				_jsonConfiguration.Settings.TestFilesBaseDirectory = Directory.GetCurrentDirectory();

			if (string.IsNullOrWhiteSpace(_jsonConfiguration.Settings.TestFilesBaseDirectory))
				_jsonConfiguration.Settings.TestFilesBaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ScriptSnippets");

			if (string.IsNullOrEmpty(_jsonConfiguration.Settings.EncryptionKey))
				_jsonConfiguration.Settings.EncryptionKey = "SyringeSyringe123";
		}

		private void EnsureEnvironments()
		{
			// TODO: Test coverage
			if (_jsonConfiguration.Environments == null || !_jsonConfiguration.Environments.Any())
				throw new ConfigurationException("The Environments section was empty or missing in the appsettings.json configuration file.");
		}

		private string ResolveRelativePath(string directoryPath)
		{
			if (string.IsNullOrEmpty(directoryPath))
			{
				return directoryPath;
			}

			if (directoryPath.StartsWith(".."))
			{
				// Convert to a relative path
				string fullPath = Path.Combine(System.AppContext.BaseDirectory, directoryPath);
				directoryPath = Path.GetFullPath(fullPath);
			}
			else
			{
				directoryPath = Path.GetFullPath(directoryPath);
			}

			return directoryPath;
		}
	}
}
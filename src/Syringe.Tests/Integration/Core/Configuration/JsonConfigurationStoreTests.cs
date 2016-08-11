using System;
using System.IO;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Tests;

namespace Syringe.Tests.Integration.Core.Configuration
{
    public class JsonConfigurationStoreTests
    {
		[Test]
        public void load_should_cache_configuration_for_next_call()
		{
			// given
			var store = new JsonConfigurationStore();
			IConfiguration config = store.Load();

			string newConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration-storetests.json");
			CopyConfigFile(newConfigPath);
			ChangeConfigFile(newConfigPath);

			// when
			IConfiguration config2 = store.Load();

			// then
			Assert.That(config, Is.EqualTo(config2));
		}

		private static void CopyConfigFile(string newConfigPath)
	    {
		    string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.json");
		    if (File.Exists(newConfigPath))
		    {
		        File.Delete(newConfigPath);
		    }

			File.Copy(configPath, newConfigPath);
	    }

		private static void ChangeConfigFile(string configPath)
		{
			string configText = File.ReadAllText(configPath);
			configText = configText.Replace("http://*:1981", "blah");
			File.WriteAllText(configPath, configText);
		}

		[Test]
        public void load_should_resolve_relative_config_file()
        {
			// given
			string expectedTestDirPath = new DirectoryInfo("..\\..\\..\\..\\Example-TestFiles").FullName;
			string expectedSnippetsDirPath = new DirectoryInfo("..\\..\\..\\..\\Example-TestFiles\\ScriptSnippets").FullName;

			var store = new JsonConfigurationStore();

	        // when
	        IConfiguration config = store.Load();

	        // then
			Assert.That(config.TestFilesBaseDirectory, Is.EqualTo(expectedTestDirPath));
			Assert.That(config.ScriptSnippetDirectory, Is.EqualTo(expectedSnippetsDirPath));
		}
	}
}
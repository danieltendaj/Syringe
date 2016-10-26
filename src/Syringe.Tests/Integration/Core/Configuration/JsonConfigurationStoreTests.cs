using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;

namespace Syringe.Tests.Integration.Core.Configuration
{
    public class JsonConfigurationStoreTests
    {
        private Mock<IConfigLocator> _configLocatorMock;

        [SetUp]
        public void Setup()
        {
            _configLocatorMock = new Mock<IConfigLocator>();
            _configLocatorMock
                .Setup(x => x.ResolveConfigFile("configuration.json"))
                .Returns(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.json"));
        }

		[Test]
        public void load_should_cache_configuration_for_next_call()
		{
			// given
			var store = new JsonConfigurationStore(_configLocatorMock.Object);
			IConfiguration config = store.Load();

			string newConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration-storetests.json");
			CopyConfigFile(newConfigPath);
			ChangeConfigFile(newConfigPath);

			// when
			IConfiguration config2 = store.Load();

			// then
			Assert.That(config, Is.EqualTo(config2));
        }

		[Test]
        public void load_should_resolve_relative_config_file()
        {
			// given
            string resolveBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\");

            string expectedTestDirPath = new DirectoryInfo(Path.Combine(resolveBasePath, "Example-TestFiles")).FullName;
			string expectedSnippetsDirPath = new DirectoryInfo(Path.Combine(resolveBasePath, "Example-TestFiles\\ScriptSnippets")).FullName;

			var store = new JsonConfigurationStore(_configLocatorMock.Object);

	        // when
	        IConfiguration config = store.Load();

	        // then
			Assert.That(config.TestFilesBaseDirectory, Is.EqualTo(expectedTestDirPath));
			Assert.That(config.ScriptSnippetDirectory, Is.EqualTo(expectedSnippetsDirPath));
        }

        public static void CopyConfigFile(string newConfigPath)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.json");
            if (File.Exists(newConfigPath))
            {
                File.Delete(newConfigPath);
            }

            File.Copy(configPath, newConfigPath);
        }

        public static void ChangeConfigFile(string configPath)
        {
            string configText = File.ReadAllText(configPath);
            configText = configText.Replace("http://*:1981", "blah");
            File.WriteAllText(configPath, configText);
        }
    }
}
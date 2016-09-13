using System;
using System.IO;
using NUnit.Framework;
using Syringe.Core.Configuration;

namespace Syringe.Tests.Integration.Core.Configuration
{
    [TestFixture]
    public class ConfigLocatorTests
    {

        [Test]
        public void resolve_file_should_detect_config_file_current_directory()
        {
            // given
            string fileName = "configuration.json";
            string expectedDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // when
            var store = new ConfigLocator();
            string path = store.ResolveConfigFile(fileName);

            // then
            Assert.That(path, Is.EqualTo(Path.Combine(expectedDirectory, fileName)));
        }

        [Test]
        public void resolve_file_should_detect_config_file_from_other_locations()
        {
            // given
            string fileName = Path.GetTempFileName();
            string expectedDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Syringe");
            string[] paths =
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Syringe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Syringe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), "Syringe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), "Syringe"),
                expectedDirectory
            };

            if (!Directory.Exists(expectedDirectory))
                Directory.CreateDirectory(expectedDirectory);

            File.Create(Path.Combine(expectedDirectory, fileName));

            // when
            var store = new ConfigLocator(paths);
            string path = store.ResolveConfigFile(fileName);

            // then
            Assert.That(path, Is.EqualTo(Path.Combine(expectedDirectory, fileName)));
        }
    }
}
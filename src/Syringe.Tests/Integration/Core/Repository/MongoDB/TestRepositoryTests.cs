using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Syringe.Core.Configuration;
using Syringe.Core.IO;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Tests.Integration.Core.Repository.MongoDB
{
    public class TestRepositoryTests
    {
        private JsonConfiguration _jsonConfiguration;
        private TestRepository _testRepository;

        [SetUp]
        public void Setup()
        {
            string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "integration", "TestRepositoryTests");
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            _jsonConfiguration = new JsonConfiguration();
            _jsonConfiguration.TestFilesBaseDirectory = baseDirectory;

            _testRepository = new TestRepository(new TestFileReader(),
                new TestFileWriter(new SerializationContract()),
                new FileHandler(_jsonConfiguration));
        }

        [Test]
        public void should()
        {
            // given
            var test = new Test()
            {
                Description = "Test description"
            };

            // when
            //bool actualResult = _testRepository.CreateTest("temptest.json", test);

            // then
            //Assert.That(actualResult, Is.EqualTo(true));
        }
    }
}
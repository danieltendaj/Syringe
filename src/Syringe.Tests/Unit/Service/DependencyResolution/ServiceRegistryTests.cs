using System;
using Moq;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.Environment.Octopus;
using Syringe.Core.IO;
using Syringe.Core.Repositories;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Service;
using Syringe.Service.DependencyResolution;
using Syringe.Service.Parallel;
using Syringe.Tests.NUnitToXUnit;
using Syringe.Tests.StubsMocks;
using Xunit;

namespace Syringe.Tests.Unit.Service.DependencyResolution
{
	public class ServiceRegistryTests
	{
		private IContainer GetContainer(IConfigurationStore store)
		{
			var registry = new ServiceRegistry(store);
			var container = new Container(c =>
			{
				c.AddRegistry(registry);
			});

			return container;
		}

		private void AssertDefaultType<TParent, TConcrete>(IContainer container = null)
		{
			// given
			if (container == null)
				container = GetContainer(new ConfigurationStoreMock());

			// when
			TParent instance = container.GetInstance<TParent>();

			// then
			Assert.IsType<TConcrete>(instance);
		}

		[Fact]
		public void should_inject_default_types()
		{
			AssertDefaultType<Startup, Startup>();
			AssertDefaultType<IConfigurationStore, ConfigurationStoreMock>(); // ConfigurationStoreMock from this test
			AssertDefaultType<IConfiguration, JsonConfiguration>();
			AssertDefaultType<IVariableEncryptor, VariableEncryptor>();

			AssertDefaultType<ITestFileResultRepository, MongoTestFileResultRepository>();
			AssertDefaultType<ITestFileQueue, ParallelTestFileQueue>();
			AssertDefaultType<ITestFileRunnerLogger, TestFileRunnerLogger>();
		}

		// TODO: services
		[Fact]
		public void configurationstore_should_be_called()
		{
			// given
			var configuration = new JsonConfiguration()
			{
				Settings = new Settings()
				{
					WebsiteUrl = "http://www.ee.i.eee.io"
				}
			};

			var configStoreMock = new Mock<IConfigurationStore>();
			configStoreMock.Setup(x => x.Load())
				.Returns(configuration)
				.Verifiable("Load wasn't called");

			IContainer container = GetContainer(configStoreMock.Object);

			// when
			var instance = container.GetInstance<IConfiguration>();

			// then
			configStoreMock.Verify(x => x.Load(), Times.Once);

			NUnitAssert.That(instance, Is.Not.Null);
			NUnitAssert.That(instance, Is.EqualTo(configuration));
		}

		[Fact]
		public void should_inject_key_for_encryption()
		{
			// given
			var configuration = new JsonConfiguration()
			{
				Settings = new Settings()
				{
					EncryptionKey = "my-password"
				}
			};

			var configStore = new ConfigurationStoreMock();
			configStore.Configuration = configuration;
			IContainer container = GetContainer(configStore);

			// when
			var encryptionInstance = container.GetInstance<IEncryption>() as AesEncryption;

			// then
			NUnitAssert.That(encryptionInstance, Is.Not.Null);
			NUnitAssert.That(encryptionInstance.Password, Is.EqualTo("my-password"));
		}

		[Fact]
		public void should_inject_context_into_testfile_repository()
		{
			// given
			IContainer container = GetContainer(new ConfigurationStoreMock());

			// when
			var instance = container.GetInstance<ITestFileResultRepositoryFactory>() as TestFileResultRepositoryFactory;

			// then
			NUnitAssert.That(instance, Is.Not.Null);
			NUnitAssert.That(instance.Context, Is.Not.Null);
			Assert.IsType<IContext>(instance.Context);
		}

		[Fact]
		public void itaskobserver_should_be_cast_to_itestfilequeue()
		{
			// given
			IContainer container = GetContainer(new ConfigurationStoreMock());

			// when
			var instance = container.GetInstance<ITaskObserver>() as ITestFileQueue;

			// then
			NUnitAssert.That(instance, Is.Not.Null);
			Assert.IsType<ITestFileQueue>(instance);
		}

		[Fact]
		public void reservedvariableprovider_should_have_placeholder_environment()
		{
			// given
			IContainer container = GetContainer(new ConfigurationStoreMock());

			// when
			var instance = container.GetInstance<IReservedVariableProvider>() as ReservedVariableProvider;

			// then
			NUnitAssert.That(instance, Is.Not.Null);
			NUnitAssert.That(instance.Environment, Is.EqualTo("<environment here>"));
		}

		[Fact]
		public void should_inject_types_for_testfile_reader_writers()
		{
			AssertDefaultType<IFileHandler, FileHandler>();
			AssertDefaultType<ITestRepository, TestRepository>();
			AssertDefaultType<ITestFileReader, TestFileReader>();
			AssertDefaultType<ITestFileWriter, TestFileWriter>();
		}

		// TODO: services
		[Fact]
		public void should_use_octopus_environment_provider_when_keys_exist()
		{
			// given
			var config = new JsonConfiguration()
			{
				Settings = new Settings()
				{
					OctopusConfiguration = new OctopusConfiguration()
					{
						OctopusApiKey = "I've got the key",
						OctopusUrl = "http://localhost"
					}
				}
			};

			var configStoreMock = new ConfigurationStoreMock();
			configStoreMock.Configuration = config;
			IContainer container = GetContainer(configStoreMock);

			// when + then
			AssertDefaultType<IOctopusRepositoryFactory, OctopusRepositoryFactory>(container);

			var octopusRepository = container.GetInstance<IOctopusRepository>() as OctopusRepository;
			Assert.NotNull(octopusRepository);

			AssertDefaultType<IEnvironmentProvider, OctopusEnvironmentProvider>(container);
		}
	}
}
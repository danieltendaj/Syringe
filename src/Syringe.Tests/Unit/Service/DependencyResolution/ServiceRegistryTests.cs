using System;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
using Syringe.Core.Tests.Variables.SharedVariables;
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
		private IContainer GetContainer(Settings settings)
		{
			var registry = new ServiceRegistry(settings);

			var container = new Container(c =>
			{
				c.AddRegistry(registry);

				// Startup.cs injects these before Structuremap
				c.AddType(typeof(IOptions<Settings>), typeof(OptionsWrapper<Settings>));
				c.AddType(typeof(IOptions<SharedVariables>), typeof(OptionsWrapper<SharedVariables>));
				c.AddType(typeof(IOptions<Syringe.Core.Environment.Environment>), typeof(OptionsWrapper<Syringe.Core.Environment.Environment>));
			});

			//container.AssertConfigurationIsValid();
			return container;
		}

		private void AssertDefaultType<TParent, TConcrete>(IContainer container = null)
		{
			// given
			if (container == null)
				container = GetContainer(new Settings() { EncryptionKey = "I was injected in the test" });

			// when
			var x = container.GetInstance<Settings>();
			TParent instance = container.GetInstance<TParent>();
			Console.WriteLine(instance.GetType());

			// then
			Assert.IsType<TConcrete>(instance);
		}

		[Fact]
		public void should_inject_default_types()
		{
			AssertDefaultType<IVariableEncryptor, VariableEncryptor>();
			AssertDefaultType<ITestFileResultRepository, PostgresTestFileResultRepository>();
			AssertDefaultType<ITestFileQueue, ParallelTestFileQueue>();
			AssertDefaultType<ITestFileRunnerLogger, TestFileRunnerLogger>();
		}

		[Fact()]//DisplayName = "IOptions", Skip = "TODO")]
		public void should_inject_ioptions()
		{
			var services = new ServiceCollection();

			var startup = new Startup(new HostingEnvironment());
			var x = startup.ConfigureServices(services);

			Console.WriteLine(x.GetService<Settings>().EncryptionKey);
		}

		[Fact]
		public void should_inject_key_for_encryption()
		{
			// given
			var settings = new Settings()
			{
				EncryptionKey = "my-password"
			};

			IContainer container = GetContainer(settings);

			// when
			var encryptionInstance = container.GetInstance<IEncryption>() as AesEncryption;

			// then
			Assert.NotNull(encryptionInstance);
			Assert.Equal("my-password", encryptionInstance.Password);
		}

		[Fact]
		public void should_inject_context_into_testfile_repository()
		{
			// given
			IContainer container = GetContainer(new Settings());

			// when
			var instance = container.GetInstance<ITestFileResultRepositoryFactory>() as TestFileResultRepositoryFactory;

			// then
			Assert.NotNull(instance);
			Assert.NotNull(instance.Context);
			Assert.IsAssignableFrom<IContext>(instance.Context);
		}

		[Fact]
		public void itaskobserver_should_be_cast_to_itestfilequeue()
		{
			// given
			IContainer container = GetContainer(new Settings());

			// when
			var instance = container.GetInstance<ITaskObserver>() as ITestFileQueue;

			// then
			Assert.NotNull(instance);
			Assert.IsAssignableFrom<ITestFileQueue>(instance);
		}

		[Fact]
		public void reservedvariableprovider_should_have_placeholder_environment()
		{
			// given
			IContainer container = GetContainer(new Settings());

			// when
			var instance = container.GetInstance<IReservedVariableProvider>() as ReservedVariableProvider;

			// then
			Assert.NotNull(instance);
			Assert.Equal("<environment here>", instance.Environment);
		}

		[Fact]
		public void should_inject_types_for_testfile_reader_writers()
		{
			AssertDefaultType<IFileHandler, FileHandler>();
			AssertDefaultType<ITestRepository, TestRepository>();
			AssertDefaultType<ITestFileReader, TestFileReader>();
			AssertDefaultType<ITestFileWriter, TestFileWriter>();
		}

		[Fact]
		public void should_use_octopus_environment_provider_when_keys_exist()
		{
			// given
			var settings = new Settings()
			{
				OctopusConfiguration = new OctopusConfiguration()
				{
					OctopusApiKey = "I've got the key",
					OctopusUrl = "http://localhost"
				}
			};

			IContainer container = GetContainer(settings);

			// when + then
			AssertDefaultType<IOctopusRepositoryFactory, OctopusRepositoryFactory>(container);

			var octopusRepository = container.GetInstance<IOctopusRepository>() as OctopusRepository;
			Assert.NotNull(octopusRepository);

			AssertDefaultType<IEnvironmentProvider, OctopusEnvironmentProvider>(container);
		}
	}
}
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.Environment.Json;
using Syringe.Core.IO;
using Syringe.Core.Repositories;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Service.Parallel;
using IConfiguration = Syringe.Core.Configuration.IConfiguration;

namespace Syringe.Service.DependencyResolution
{
	public class ServiceRegistry : Registry
	{
		public ServiceRegistry()
		{
			Scan(
				scan =>
				{
					scan.TheCallingAssembly();
					scan.Assembly("Syringe.Core");
					scan.WithDefaultConventions();
				});

			For<Startup>().Use<Startup>().Singleton();

			For<IConfigurationStore>().Use(x => new JsonConfigurationStore()).Singleton();
			For<IConfiguration>().Use(x => x.GetInstance<IConfigurationStore>().Load()).Singleton();

			For<IEncryption>().Use(x => new AesEncryption(x.GetInstance<IConfiguration>().Settings.EncryptionKey));
			For<IVariableEncryptor>().Use<VariableEncryptor>();

			// ParallelTestFileQueue dependencies
			For<ITestFileRunnerLoggerFactory>().Use<TestFileRunnerLoggerFactory>().Singleton();
			For<ITestFileResultRepositoryFactory>().Use(ctx => new TestFileResultRepositoryFactory(ctx));
			For<ITestFileResultRepository>().Use<PostgresTestFileResultRepository>().Singleton();
			For<ITestFileQueue>().Use<ParallelTestFileQueue>().Singleton();

			Forward<ITestFileQueue, ITaskObserver>();

			For<IBatchManager>().Use<BatchManager>().Singleton();
			For<IReservedVariableProvider>().Use(() => new ReservedVariableProvider("<environment here>"));

			SetupTestFileFormat();
			SetupEnvironmentSource(new JsonConfiguration());
			For<IEnvironmentProvider>().Use<JsonEnvironmentProvider>();

			//TODO?
			//For<ObjectCache>().Use(x => MemoryCache.Default);
		}

		internal void SetupEnvironmentSource(IConfiguration configuration)
		{
			// Environments, use Octopus if keys exist
			//bool containsOctopusApiKey = !string.IsNullOrEmpty(configuration.OctopusConfiguration?.OctopusApiKey);
			//bool containsOctopusUrl = !string.IsNullOrEmpty(configuration.OctopusConfiguration?.OctopusUrl);

			//if (containsOctopusApiKey && containsOctopusUrl)
			//{
			//    For<IOctopusRepositoryFactory>().Use<OctopusRepositoryFactory>();
			//    For<IOctopusRepository>().Use(x => x.GetInstance<IOctopusRepositoryFactory>().Create());
			//    For<IEnvironmentProvider>().Use<OctopusEnvironmentProvider>().Singleton();
			//}
			//else
			//{
			For<IEnvironmentProvider>().Use<JsonEnvironmentProvider>();
			//}
		}

		private void SetupTestFileFormat()
		{
			For<IFileHandler>().Use<FileHandler>();
			For<ITestRepository>().Use<TestRepository>();
			For<ITestFileReader>().Use<TestFileReader>();
			For<ITestFileWriter>().Use<TestFileWriter>();
		}
	}
}
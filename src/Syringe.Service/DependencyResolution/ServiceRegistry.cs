using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.Environment.Json;
using Syringe.Core.Environment.Octopus;
using Syringe.Core.Http;
using Syringe.Core.IO;
using Syringe.Core.Repositories;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Core.Tests.Variables.SharedVariables;
using Syringe.Service.Parallel;
using IConfiguration = Syringe.Core.Configuration.IConfiguration;

namespace Syringe.Service.DependencyResolution
{
	public class ServiceRegistry : Registry
	{
		public ServiceRegistry(IConfigurationStore configurationStore)
		{
			Scan(
				scan =>
				{
					scan.AssemblyContainingType(typeof(Startup));
					scan.Assembly("Syringe.Core");
					scan.WithDefaultConventions();
				});

			For<IConfigurationStore>().Use(configurationStore).Singleton();
			For<IConfiguration>().Use(x => x.GetInstance<IConfigurationStore>().Load()).Singleton();

			For<HttpClient>().Use(new HttpClient()).Singleton();
			For<IHttpClientAdapter>().Use(x => new HttpClientAdapter(x.GetInstance<HttpClient>()));

			For<IEncryption>().Use("IEncryption", x =>
			{
				// Get the encryption key
				var config = x.GetInstance<IConfiguration>();
				string key = config.Settings.EncryptionKey;

				return new AesEncryption(key);
			}).Singleton();

			For<IVariableEncryptor>().Use<VariableEncryptor>();

			// ParallelTestFileQueue dependencies
			For<ITestFileRunnerLoggerFactory>().Use<TestFileRunnerLoggerFactory>().Singleton();
			For<ITestFileResultRepositoryFactory>().Use(ctx => new TestFileResultRepositoryFactory(ctx));
			For<ITestFileResultRepository>().Use<PostgresTestFileResultRepository>().Singleton();
			For<ITestFileQueue>().Use<ParallelTestFileQueue>().Singleton();

			Forward<ITestFileQueue, ITaskObserver>();

			For<IBatchManager>().Use<BatchManager>().Singleton();
			For<IReservedVariableProvider>().Use(() => new ReservedVariableProvider("<environment here>"));

			// TODO: how do we get IOptions from .NET?
			For<IOptions<SharedVariables>>().Use(() => new OptionsWrapper<SharedVariables>(new SharedVariables()));

			SetupTestFileFormat();
			SetupEnvironmentSource(configurationStore.Load());

			For<IMemoryCache>().Use(new MemoryCache(new MemoryCacheOptions()));
		}

		internal void SetupEnvironmentSource(IConfiguration configuration)
		{
			// Environments, use Octopus if keys exist
			bool containsOctopusApiKey = !string.IsNullOrEmpty(configuration.Settings?.OctopusConfiguration?.OctopusApiKey);
			bool containsOctopusUrl = !string.IsNullOrEmpty(configuration.Settings?.OctopusConfiguration?.OctopusUrl);

			if (containsOctopusApiKey && containsOctopusUrl)
			{
				For<IOctopusRepositoryFactory>().Use<OctopusRepositoryFactory>();
				For<IOctopusRepository>().Use(x => x.GetInstance<IOctopusRepositoryFactory>().Create());
				For<IEnvironmentProvider>().Use<OctopusEnvironmentProvider>().Singleton();
			}
			else
			{
				For<IEnvironmentProvider>().Use<JsonEnvironmentProvider>();
			}
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
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.Environment.Json;
using Syringe.Core.Environment.Octopus;
using Syringe.Core.IO;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Repositories.Json.Reader;
using Syringe.Core.Tests.Repositories.Json.Writer;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Core.Tests.Variables.ReservedVariables;
using Syringe.Service.Parallel;

namespace Syringe.Service.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry() : this(null)
        { }

        public DefaultRegistry(IConfigurationStore configurationStore)
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.Assembly("Syringe.Core");
                    scan.WithDefaultConventions();
                });

            For<Startup>().Use<Startup>().Singleton();

            // Configuration: load the configuration from the store
            if (configurationStore == null)
            {
                configurationStore = new JsonConfigurationStore(new ConfigLocator());
            }

            For<IConfigurationStore>().Use(configurationStore).Singleton();

            IConfiguration configuration = configurationStore.Load();
            For<IConfiguration>().Use(configuration);

            For<IEncryption>().Use(x => new AesEncryption(x.GetInstance<IConfiguration>().EncryptionKey));
            For<IVariableEncryptor>().Use<VariableEncryptor>();

            // ParallelTestFileQueue dependencies
            For<ITestFileRunnerLoggerFactory>().Use<TestFileRunnerLoggerFactory>().Singleton();
            For<ITestFileResultRepositoryFactory>().Use(ctx => new TestFileResultRepositoryFactory(ctx));
            For<ITestFileResultRepository>().Use<MongoTestFileResultRepository>().Singleton();
            For<ITestFileQueue>().Use<ParallelTestFileQueue>().Singleton();

            Forward<ITestFileQueue, ITaskObserver>();

            For<IBatchManager>().Use<BatchManager>().Singleton();
            For<IReservedVariableProvider>().Use(() => new ReservedVariableProvider("<environment here>"));

            SetupTestFileFormat();
            SetupEnvironmentSource(configuration);

            //TODO
            //For<ObjectCache>().Use(x => MemoryCache.Default);
        }

        internal void SetupEnvironmentSource(IConfiguration configuration)
        {
            // Environments, use Octopus if keys exist
            bool containsOctopusApiKey = !string.IsNullOrEmpty(configuration.OctopusConfiguration?.OctopusApiKey);
            bool containsOctopusUrl = !string.IsNullOrEmpty(configuration.OctopusConfiguration?.OctopusUrl);

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
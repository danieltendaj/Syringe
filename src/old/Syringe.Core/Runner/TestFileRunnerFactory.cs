using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Core.Runner
{
    public class TestFileRunnerFactory : ITestFileRunnerFactory
    {
        private readonly ITestFileResultRepositoryFactory _repositoryFactory;
        private readonly IConfiguration _configuration;
        private readonly ICapturedVariableProviderFactory _capturedVariableProviderFactory;
        private readonly ITestFileRunnerLoggerFactory _loggerFactory;

        public TestFileRunnerFactory(ITestFileResultRepositoryFactory repositoryFactory, IConfiguration configuration, 
            ICapturedVariableProviderFactory capturedVariableProviderFactory, ITestFileRunnerLoggerFactory loggerFactory)
        {
            _repositoryFactory = repositoryFactory;
            _configuration = configuration;
            _capturedVariableProviderFactory = capturedVariableProviderFactory;
            _loggerFactory = loggerFactory;
        }

        public TestFileRunner Create()
        {
            var httpClient = new HttpClient(new RestClient());
            return new TestFileRunner(httpClient, _repositoryFactory, _configuration, _capturedVariableProviderFactory, _loggerFactory);
        }
    }
}
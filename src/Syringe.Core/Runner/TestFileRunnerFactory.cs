using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Core.Runner
{
    public class TestFileRunnerFactory : ITestFileRunnerFactory
    {
        private readonly ITestFileResultRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ICapturedVariableProviderFactory _capturedVariableProviderFactory;

        public TestFileRunnerFactory(ITestFileResultRepository repository, IConfiguration configuration, ICapturedVariableProviderFactory capturedVariableProviderFactory)
        {
            _repository = repository;
            _configuration = configuration;
            _capturedVariableProviderFactory = capturedVariableProviderFactory;
        }

        public TestFileRunner Create()
        {
            var httpClient = new HttpClient(new RestClient());
            return new TestFileRunner(httpClient, _repository, _configuration, _capturedVariableProviderFactory);
        }
    }
}
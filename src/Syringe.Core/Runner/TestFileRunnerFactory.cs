using System.Net.Http;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Runner.Logging;

namespace Syringe.Core.Runner
{
	public class TestFileRunnerFactory : ITestFileRunnerFactory
	{
		private readonly ITestFileResultRepositoryFactory _repositoryFactory;
		private readonly Settings _settings;
		private readonly ICapturedVariableProviderFactory _capturedVariableProviderFactory;
		private readonly ITestFileRunnerLoggerFactory _loggerFactory;
		private readonly HttpClient _httpClient;

		public TestFileRunnerFactory(ITestFileResultRepositoryFactory repositoryFactory, Settings settings,
			ICapturedVariableProviderFactory capturedVariableProviderFactory, ITestFileRunnerLoggerFactory loggerFactory, HttpClient httpClient)
		{
			_repositoryFactory = repositoryFactory;
			_settings = settings;
			_capturedVariableProviderFactory = capturedVariableProviderFactory;
			_loggerFactory = loggerFactory;
			_httpClient = httpClient;
		}

		public TestFileRunner Create()
		{
			var httpClientAdapter = new HttpClientAdapter(_httpClient);
			return new TestFileRunner(httpClientAdapter, _repositoryFactory, _settings, _capturedVariableProviderFactory, _loggerFactory);
		}
	}
}
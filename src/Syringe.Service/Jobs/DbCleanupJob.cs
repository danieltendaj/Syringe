using System;
using System.Threading.Tasks;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Service.Jobs
{
    public class DbCleanupJob : IDbCleanupJob
    {
        private readonly IConfiguration _configuration;
        private readonly ITestFileResultRepository _repository;

        public DbCleanupJob(IConfiguration configuration, ITestFileResultRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public void Start()
        {
        }

        internal void Cleanup()
        {
            DateTime cleanupBefore = DateTime.Today.AddDays(-_configuration.DaysOfDataRetention);
            _repository.DeleteBeforeDate(cleanupBefore).Wait();
        }
    }
}
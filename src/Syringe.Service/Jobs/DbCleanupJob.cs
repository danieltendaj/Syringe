using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Service.Jobs
{
    public class DbCleanupJob : IDbCleanupJob
    {
        private readonly ITestFileResultRepository _repository;
        public DbCleanupJob(ITestFileResultRepository repository)
        {
            _repository = repository;
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
using System;
using System.Threading;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Service.Jobs
{
    public class DbCleanupJob : IDbCleanupJob
    {
        private readonly IConfiguration _configuration;
        private readonly ITestFileResultRepository _repository;
        private Timer _timer;

        public DbCleanupJob(IConfiguration configuration, ITestFileResultRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public void Start()
        {
            Start(Cleanup);
        }

        internal void Start(TimerCallback callback)
        {
            if (_timer == null)
            {
                _timer = new Timer(callback, null, new TimeSpan(), _configuration.CleanupSchedule);
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        internal void Cleanup(object guff)
        {
            DateTime cleanupBefore = DateTime.Today.AddDays(-_configuration.DaysOfDataRetention);
            _repository.DeleteBeforeDate(cleanupBefore).Wait();
        }
    }
}
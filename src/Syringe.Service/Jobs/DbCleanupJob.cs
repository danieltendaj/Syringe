using System;
using System.Threading;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories;

namespace Syringe.Service.Jobs
{
	public class DbCleanupJob : IDbCleanupJob
	{
		private readonly Settings _settings;
		private readonly ITestFileResultRepository _repository;
		private Timer _timer;

		public DbCleanupJob(Settings settings, ITestFileResultRepository repository)
		{
			_settings = settings;
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
				_timer = new Timer(callback, null, new TimeSpan(), _settings.CleanupSchedule);
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
			DateTime cleanupBefore = DateTime.Today.AddDays(-_settings.DaysOfDataRetention);
			_repository.DeleteBeforeDate(cleanupBefore).Wait();
		}
	}
}
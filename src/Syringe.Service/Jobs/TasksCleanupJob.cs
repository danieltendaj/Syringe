using System;
using System.Collections.Generic;
using System.Threading;
using Syringe.Core.Configuration;
using Syringe.Core.Tasks;
using Syringe.Service.Parallel;

namespace Syringe.Service.Jobs
{
    public class TasksCleanupJob : IJob
    {
        internal readonly TimeSpan _retention = TimeSpan.FromHours(2);
        private readonly IConfiguration _configuration;
        private readonly ITestFileQueue _testFileQueue;
        private Timer _timer;

        public TasksCleanupJob(IConfiguration configuration, ITestFileQueue testFileQueue)
        {
            _configuration = configuration;
            _testFileQueue = testFileQueue;
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
            var toRemove = new List<int>();
            DateTime earliestTime = DateTime.UtcNow.Subtract(_retention);

            foreach (TaskDetails task in _testFileQueue.GetRunningTasks())
            {
                if (task.IsComplete && task.StartTime.ToUniversalTime() < earliestTime)
                {
                    toRemove.Add(task.TaskId);
                }
            }

            toRemove.ForEach(_testFileQueue.Remove);
        }
    }
}
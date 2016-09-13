using System;
using System.Collections.Generic;
using System.Threading;
using Syringe.Core.Tasks;
using Syringe.Service.Parallel;

namespace Syringe.Service.Jobs
{
    public class TasksCleanup : IJob
    {
        private readonly ITestFileQueue _testFileQueue;
        private Timer _timer;

        public TasksCleanup(ITestFileQueue testFileQueue)
        {
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
                _timer = new Timer(callback, null, new TimeSpan(), TimeSpan.FromMinutes(1));
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

            foreach (TaskDetails task in _testFileQueue.GetRunningTasks())
            {
                if (task.IsComplete && task.StartTime < DateTime.UtcNow.AddHours(-1))
                {
                    toRemove.Add(task.TaskId);
                }
            }

            toRemove.ForEach(_testFileQueue.Remove);
        }
    }
}
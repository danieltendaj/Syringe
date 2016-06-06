using System;
using System.Threading;
using System.Threading.Tasks;
using Syringe.Core.Runner;
using Syringe.Core.Tasks;
using Syringe.Core.Tests.Results;

namespace Syringe.Service.Parallel
{
    public class TestFileRunnerTaskInfo
    {
        public TestFileRunnerTaskInfo(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string Username { get; set; }
        public DateTime StartTime { get; set; }
        public TaskRequest Request { get; set; }
        public TestFileRunner Runner { get; set; }

        public CancellationTokenSource CancelTokenSource { get; set; }
        public Task CurrentTask { get; set; }

        public string Errors { get; set; }

        public int? Position { get; set; }
        public TestFileResult TestFileResults { get; set; }
    }
}
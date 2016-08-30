using System.Collections.Generic;
using Syringe.Core.Tasks;
using Syringe.Service.Parallel;

namespace Syringe.Tests.StubsMocks
{
    public class TestFileQueueStub : ITestFileQueue
    {
        public int TaskId { get; set; }
        public List<TaskRequest> Add_Tasks { get; set; } = new List<TaskRequest>();
        public int Add(TaskRequest item)
        {
            Add_Tasks.Add(item);
            TaskId++;
            return TaskId;
        }

        public IEnumerable<TaskDetails> GetRunningTasks()
        {
            throw new System.NotImplementedException();
        }

        public TaskDetails GetRunningTaskDetails(int taskId)
        {
            throw new System.NotImplementedException();
        }

        public TestFileRunnerTaskInfo GetTestFileTaskInfo(int taskId)
        {
            throw new System.NotImplementedException();
        }
    }
}
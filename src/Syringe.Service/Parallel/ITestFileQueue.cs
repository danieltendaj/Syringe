using System.Collections.Generic;
using Syringe.Core.Tasks;

namespace Syringe.Service.Parallel
{
    public interface ITestFileQueue
    {
        /// <summary>
        /// Adds a request to run a test file the queue of tasks to run.
        /// </summary>
        int Add(TaskRequest item);

        /// <summary>
        /// Shows minimal information about all test file requests in the queue, and their status,
        /// and who started the run.
        /// </summary>
        IEnumerable<TaskDetails> GetRunningTasks();

        /// <summary>
        /// Shows the full information about a *single* test run - it doesn't have to be running, it could be complete.
        /// This includes the results of every test in the test file for the run.
        /// </summary>
        TaskDetails GetRunningTaskDetails(int taskId);

        TestFileRunnerTaskInfo GetTestFileTaskInfo(int taskId);

        /// <summary>
        /// Will remove task from list/memory
        /// </summary>
        void Remove(int taskId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Tasks;
using Syringe.Service.Parallel;

namespace Syringe.Service
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

        /// <summary>
        /// Stops a test file request task in the queue, returning a message of whether the stop succeeded or not.
        /// </summary>
        string Stop(int id);

        /// <summary>
        /// Attempts to shut down all running tasks.
        /// </summary>
        List<string> StopAll();

	    /// <summary>
	    /// Runs a test file and waits.
	    /// </summary>
	    Task<TestFileRunnerTaskInfo> RunAsync(TaskRequest request);
    }
}
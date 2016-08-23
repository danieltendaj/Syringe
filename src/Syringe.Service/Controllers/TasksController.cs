using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Service.Models;
using Syringe.Service.Parallel;

namespace Syringe.Service.Controllers
{
    public class TasksController : ApiController, ITasksService
    {
        private readonly ITestFileQueue _fileQueue;
        private readonly ITestFileResultFactory _testFileResultFactory;

        public TasksController(ITestFileQueue fileQueue, ITestFileResultFactory testFileResultFactory)
        {
            _fileQueue = fileQueue;
            _testFileResultFactory = testFileResultFactory;
        }

        [Route("api/task")]
        [HttpGet]
        public TaskDetails GetTask(int taskId)
        {
            return _fileQueue.GetRunningTaskDetails(taskId);
        }

        [Route("api/task")]
        [HttpPost]
        public int Start(TaskRequest item)
        {
            return _fileQueue.Add(item);
        }

        [Route("api/tasks")]
        [HttpGet]
        public IEnumerable<TaskDetails> GetTasks()
        {
            return _fileQueue.GetRunningTasks();
        }

        /// <summary>
        /// Run a test file synchronously - waiting for the tests to finish.
        /// </summary>
        [Route("api/task/runTestFile")]
        [HttpGet]
        public TestFileRunResult RunTestFile(string filename, string environment, string username)
        {
            DateTime startTime = DateTime.UtcNow;

            var taskRequest = new TaskRequest()
            {
                Environment = environment,
                Filename = filename,
                Username = username
            };

            try
            {
                // Wait 2 minutes for the tests to run, this can be made configurable later
                TimeSpan timeout = TimeSpan.FromMinutes(2);
                Task<TestFileRunnerTaskInfo> task = _fileQueue.RunAsync(taskRequest);
                bool completed = task.Wait(timeout);
                TimeSpan timeTaken = DateTime.UtcNow - startTime;

                return _testFileResultFactory.Create(task, !completed, timeTaken);
            }
            catch (Exception ex)
            {
                TimeSpan timeTaken = DateTime.UtcNow - startTime;

                // Error
                return new TestFileRunResult()
                {
                    Completed = false,
                    TimeTaken = timeTaken,
                    ErrorMessage = ex.ToString()
                };
            }
        }
    }
}
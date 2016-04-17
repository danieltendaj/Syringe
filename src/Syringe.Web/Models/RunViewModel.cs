using System;
using System.Collections.Generic;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;

namespace Syringe.Web.Models
{
    public class RunViewModel : IRunViewModel
    {
        private readonly ITasksService _tasksService;
        private readonly ITestService _testService;
        private readonly List<RunningTestViewModel> _runningTests = new List<RunningTestViewModel>();

		public IEnumerable<RunningTestViewModel> Tests => _runningTests;
		public int CurrentTaskId { get; private set; }
		public string FileName { get; private set; }
		public string SignalRUrl { get; private set; }

		public RunViewModel(ITasksService tasksService, ITestService testService)
        {
            _tasksService = tasksService;
            _testService = testService;

	        var mvcConfiguration = new MvcConfiguration();
			SignalRUrl = mvcConfiguration.SignalRUrl;
        }

        public void RunTest(IUserContext userContext, string fileName, int index)
        {
            FileName = fileName;

            Test test = _testService.GetTest(fileName,  index);

            var verifications = new List<Assertion>();
            verifications.AddRange(test.Assertions);
            _runningTests.Add(new RunningTestViewModel(test.Position, test.Description, verifications));

            var taskRequest = new TaskRequest
            {
                Filename = fileName,
                Username = userContext.FullName,
                BranchName = userContext.DefaultBranchName,
                Position = index,
            };

            CurrentTaskId = _tasksService.Start(taskRequest);
        }


        public void Run(IUserContext userContext, string fileName)
        {
            FileName = fileName;

            TestFile testFile = _testService.GetTestFile(fileName, userContext.DefaultBranchName);

            foreach (Test test in testFile.Tests)
            {
                var verifications = new List<Assertion>();
                verifications.AddRange(test.Assertions);
                _runningTests.Add(new RunningTestViewModel(test.Position, test.Description, verifications));
            }

            var taskRequest = new TaskRequest
            {
                Filename = fileName,
                Username = userContext.FullName,
                BranchName = userContext.DefaultBranchName,
            };

            CurrentTaskId = _tasksService.Start(taskRequest);
        }
    }
}
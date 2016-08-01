using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Environment { get; private set; }
        public string SignalRUrl { get; private set; }

		public RunViewModel(ITasksService tasksService, ITestService testService, MvcConfiguration mvcConfiguration)
        {
            _tasksService = tasksService;
            _testService = testService;

			SignalRUrl = mvcConfiguration.SignalRUrl;
        }

        public void Run(IUserContext userContext, string fileName, string environment)
        {
            FileName = fileName;
            Environment = environment;

            TestFile testFile = _testService.GetTestFile(fileName);

            for (int i = 0; i < testFile.Tests.Count(); i++)
            {
                Test test = testFile.Tests.ElementAt(i);
                var verifications = new List<Assertion>();
                verifications.AddRange(test.Assertions);
                _runningTests.Add(new RunningTestViewModel(i, test.Description, verifications));
            }
            
            var taskRequest = new TaskRequest
            {
                Filename = fileName,
                Username = userContext.FullName,
                Environment = environment
            };

            CurrentTaskId = _tasksService.Start(taskRequest);
        }
    }
}
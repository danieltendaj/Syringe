using System;
using System.Linq;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Web;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
	[TestFixture]
	public class RunViewModelTests
	{
		private static RunViewModel GivenARunViewModel(ITasksService tasksService = null, ITestService testService = null)
		{
			return new RunViewModel(
				tasksService ?? Mock.Of<ITasksService>(),
				testService ?? Mock.Of<ITestService>());
		}

		[TestFixture]
		public class when_running
		{
			[Test]
			public void should_set_filename_to_specified_file()
			{
				// given
				const string fileName = "Some file";
				var testService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<TestFile>());

				var viewModel = GivenARunViewModel(testService: testService);

				// when
				viewModel.Run(Mock.Of<IUserContext>(), fileName);

				// then
				Assert.That(viewModel.FileName, Is.EqualTo(fileName));
			}

			[Test]
			public void should_populate_tests_using_specified_filename()
			{
				// given
				const string fileName = "Some file";
				const string branchName = "master";
			    int test1 = 1;
			    int test2 = 2;
				var testFile =
					Mock.Of<TestFile>(
						c =>
							c.Tests ==
							new[]
							{
								new Test { Position = test1, Description = "Desc1"},
								new Test { Position = test2, Description = "Desc2"}
							});

				var testService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(fileName, branchName) == testFile);

				var viewModel = GivenARunViewModel(testService: testService);

				// when
				viewModel.Run(Mock.Of<IUserContext>(c => c.DefaultBranchName == branchName), fileName);

				// then
				Assert.That(viewModel.Tests, Is.Not.Null);
				Assert.That(viewModel.Tests.Select(c => new { Position = c.Position, c.Description }), Is.EquivalentTo(new[]
				{
					new { Position = test1, Description = "Desc1" },
					new { Position = test2, Description = "Desc2" }
				}));
			}

			[Test]
			public void should_start_task_using_current_user_context_and_file_name()
			{
				// given
				const string fileName = "MyFile";
				const string userName = "Me";
				const string branchName = "mdstertert";

				var testService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<TestFile>());

				var tasksService = new Mock<ITasksService>();

				var viewModel = GivenARunViewModel(testService: testService, tasksService: tasksService.Object);

				// when
				viewModel.Run(Mock.Of<IUserContext>(c => c.DefaultBranchName == branchName && c.FullName == userName), fileName);

				// then
				tasksService.Verify(
					s =>
						s.Start(
							It.Is<TaskRequest>(
								r => r.BranchName == branchName && r.Filename == fileName && r.Username == userName)),
					"Should have requested for the correct task to start.");
			}

			[Test]
			public void should_set_current_task_id_to_running_task_id()
			{
				// given
				const int taskId = 121;
				var testService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<TestFile>());

				var tasksService = Mock.Of<ITasksService>(s => s.Start(It.IsAny<TaskRequest>()) == taskId);
				var viewModel = GivenARunViewModel(testService: testService, tasksService: tasksService);

				// when
				viewModel.Run(Mock.Of<IUserContext>(), "My test file");

				// then
				Assert.That(viewModel.CurrentTaskId, Is.EqualTo(taskId));
			}
		}
	}
}

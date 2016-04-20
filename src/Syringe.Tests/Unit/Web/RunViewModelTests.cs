using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
	public static class RunViewModelTests
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
				ITestService testService = Mock.Of<ITestService>(s => s.GetTestFile(It.IsAny<string>()) == Mock.Of<TestFile>());
				RunViewModel viewModel = GivenARunViewModel(testService: testService);

				// when
				viewModel.Run(Mock.Of<IUserContext>(), fileName, null);

				// then
				Assert.That(viewModel.FileName, Is.EqualTo(fileName));
			}

			[Test]
			public void should_populate_tests_using_specified_filename()
			{
				// given
				const string fileName = "Some file";
			    const string environment = "hardcore xxx environment";
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

				ITestService testService = Mock.Of<ITestService>(s => s.GetTestFile(fileName) == testFile);
                RunViewModel viewModel = GivenARunViewModel(testService: testService);

				// when
				viewModel.Run(Mock.Of<IUserContext>(), fileName, environment);

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
			    const string environment = "Chris loves a bit of environment if you know what I mean...";
				const string userName = "Me";

				ITestService testService =Mock.Of<ITestService>(s => s.GetTestFile(It.IsAny<string>()) == Mock.Of<TestFile>());
				var tasksService = new Mock<ITasksService>();
				RunViewModel viewModel = GivenARunViewModel(testService: testService, tasksService: tasksService.Object);

				// when
				viewModel.Run(Mock.Of<IUserContext>(c => c.FullName == userName), fileName, environment);

				// then
				tasksService.Verify(
					s =>
						s.Start(
							It.Is<TaskRequest>(
								r => r.Filename == fileName && r.Username == userName && r.Environment == environment)),
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
							s.GetTestFile(It.IsAny<string>()) == Mock.Of<TestFile>());

				var tasksService = Mock.Of<ITasksService>(s => s.Start(It.IsAny<TaskRequest>()) == taskId);
				var viewModel = GivenARunViewModel(testService: testService, tasksService: tasksService);

				// when
				viewModel.Run(Mock.Of<IUserContext>(), "My test file", "something");

				// then
				Assert.That(viewModel.CurrentTaskId, Is.EqualTo(taskId));
			}
		}
	}
}

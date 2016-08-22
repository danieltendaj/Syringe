using System;
using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Tasks;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Service;

namespace Syringe.Tests.Integration.ClientAndService
{
    [TestFixture]
    public class TaskClientTests
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ServiceStarter.StartSelfHostedOwin();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            ServiceStarter.StopSelfHostedOwin();
        }

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Wiping db results database");
            ServiceStarter.Container.GetInstance<ITestFileResultRepository>().Wipe();

            ServiceStarter.RecreateTestFileDirectory();
        }

        [Test]
        public void should_return_running_tasks()
        {
            // given
            var client = Helpers.CreateTasksClient();

            // when
            IEnumerable<TaskDetails> runningTasks = client.GetTasks();

            // then
            Assert.That(runningTasks, Is.Not.Null);
        }

        [Test]
        public void should_return_running_task()
        {
            // given
            var taskRequest = new TaskRequest { Environment = "anything", Filename = "test-test.json" };
            ServiceStarter.Container.GetInstance<ITestFileQueue>().Add(taskRequest);
            var client = Helpers.CreateTasksClient();
            
            // when
            TaskDetails task = client.GetTask(1);

            // then
            Assert.That(task, Is.Not.Null);
        }
    }
}
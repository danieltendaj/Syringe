using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Client;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Core.Tests.Variables;

namespace Syringe.Tests.Integration.ClientAndService
{
    [TestFixture]
    public class TestsClientTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ServiceStarter.StartSelfHostedOwin();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ServiceStarter.StopSelfHostedOwin();
        }

        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Wiping db results database");
            ServiceStarter.Container.GetInstance<ITestFileResultRepository>().Wipe().Wait();

            ServiceStarter.RecreateTestFileDirectory();
        }

        [Test]
        public void ListFiles_should_list_all_files()
        {
            // given
            string testFilepath1 = Helpers.GetFullPath(Helpers.GetJsonFilename());
            File.WriteAllText(testFilepath1, @"ANY DATA1");

            string testFilepath2 = Helpers.GetFullPath(Helpers.GetJsonFilename());
            File.WriteAllText(testFilepath2, @"ANY DATA2");

            TestsClient client = Helpers.CreateTestsClient();

            // when
            IEnumerable<string> files = client.ListFiles();

            // then
            Assert.That(files, Is.Not.Null);
            Assert.That(files.Count(), Is.EqualTo(2));
        }
        
        [Test]
        public void GetTestFile_should_return_expected_testfile()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);

            // when
            TestFile actualTestFile = client.GetTestFile(testFile.Filename);

            // then
            Assert.That(actualTestFile, Is.Not.Null);
            Assert.That(actualTestFile.Filename, Is.EqualTo(testFile.Filename));
            Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetRawFile_should_return_expected_source()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);

            // when
            string rawFile = client.GetRawFile(testFile.Filename);

            // then
            Assert.That(rawFile, Is.Not.Null);
            Assert.That(rawFile, Does.Contain(@"""Tests"": ["));
        }

        [Test]
        public void EditTest_should_save_changes_to_test_as_expected()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);
            Test expectedTest = testFile.Tests.FirstOrDefault();
            expectedTest.Description = "new description";

            // when
            bool success = client.EditTest(testFile.Filename, 0, expectedTest);

            // then
            TestFile actualTest = client.GetTestFile(testFile.Filename);

            Assert.True(success);
            Assert.That(actualTest.Tests.First().Description, Does.Contain("new description"));
        }

        [Test]
        public void CreateTest_should_create_test_for_existing_file()
        {
            // given
            string filename = Helpers.GetJsonFilename();
            TestsClient client = Helpers.CreateTestsClient();
            client.CreateTestFile(new TestFile() { Filename = filename });

            var test = new Test()
            {
                Assertions = new List<Assertion>(),
                AvailableVariables = new List<Variable>(),
                CapturedVariables = new List<CapturedVariable>(),
                Headers = new List<HeaderItem>(),
                Method = "POST",
                Url = "url"
            };

            // when
            bool success = client.CreateTest(filename, test);

            // then
            string fullPath = Helpers.GetFullPath(filename);

            Assert.True(success);
            Assert.True(File.Exists(fullPath));
            Assert.That(new FileInfo(fullPath).Length, Is.GreaterThan(0));
        }

        [Test]
        public void DeleteTest_should_save_changes_to_test()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile expectedTestFile = Helpers.CreateTestFileAndTest(client);

            // when
            bool success = client.DeleteTest(0, expectedTestFile.Filename);

            // then
            TestFile actualTestFile = client.GetTestFile(expectedTestFile.Filename);

            Assert.True(success);
            Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateTestFile_should_write_file()
        {
            // given
            string filename = Helpers.GetJsonFilename();
            TestsClient client = Helpers.CreateTestsClient();

            // when
            bool success = client.CreateTestFile(new TestFile() { Filename = filename });

            // then
            string fullPath = Helpers.GetFullPath(filename);

            Assert.True(success);
            Assert.True(File.Exists(fullPath));
            Assert.That(new FileInfo(fullPath).Length, Is.GreaterThan(0));
        }

        //--------------------------
        [Test]
        public void UpdateTestFile_should_store_changes()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);
            testFile.Tests = new List<Test>();

            // when
            bool success = client.UpdateTestVariables(testFile);

            // then
            Assert.True(success);

            TestFile actualTestFile = client.GetTestFile(testFile.Filename);
            Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetSummaries_should_return_all_results()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);

            var repository = ServiceStarter.Container.GetInstance<ITestFileResultRepository>();
            var result1 = new TestFileResult()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1),
                Filename = testFile.Filename
            };
            var result2 = new TestFileResult()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1),
                Filename = testFile.Filename
            };

            repository.Add(result1).Wait();
            repository.Add(result2).Wait();

            // when
            TestFileResultSummaryCollection results = client.GetSummaries(It.IsAny<DateTime>()).Result;

            // then
            Assert.That(results.TotalFileResults, Is.EqualTo(2));
        }

        [Test]
        public void GetResultById_should_return_expected_result()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);

            var repository = ServiceStarter.Container.GetInstance<ITestFileResultRepository>();
            var result1 = new TestFileResult()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1),
                Filename = testFile.Filename
            };
            var result2 = new TestFileResult()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1),
                Filename = testFile.Filename
            };

            repository.Add(result1).Wait();
            repository.Add(result2).Wait();

            // when
            TestFileResult actualResult = client.GetResultById(result2.Id);

            // then
            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult.Id, Is.EqualTo(result2.Id));
        }

        [Test]
        public void DeleteResultAsync_should_delete_expected_result()
        {
            // given
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);

            var repository = ServiceStarter.Container.GetInstance<ITestFileResultRepository>();
            var result1 = new TestFileResult()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1),
                Filename = testFile.Filename
            };
            var result2 = new TestFileResult()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(1),
                Filename = testFile.Filename
            };

            repository.Add(result1).Wait();
            repository.Add(result2).Wait();

            // when
            var result = client.DeleteResult(result2.Id);

            // then
            Assert.That(result, Is.True);
            TestFileResult deletedResult = client.GetResultById(result2.Id);
            Assert.That(deletedResult, Is.Null);

            TestFileResult otherResult = client.GetResultById(result1.Id);
            Assert.That(otherResult, Is.Not.Null);
        }

        [Test]
        public void DeleteFile_should_delete_file_from_disk()
        {
            // given
            string filename = Helpers.GetJsonFilename();
            TestsClient client = Helpers.CreateTestsClient();
            TestFile testFile = Helpers.CreateTestFileAndTest(client);

            // when
            bool success = client.DeleteFile(testFile.Filename);

            // then
            string fullPath = Helpers.GetFullPath(filename);

            Assert.True(success);
            Assert.False(File.Exists(fullPath));
        }
    }
}

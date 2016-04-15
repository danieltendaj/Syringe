using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Integration.ClientAndService
{
	[TestFixture]
	public class TestsClientTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ServiceConfig.StartSelfHostedOwin();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ServiceConfig.OwinServer.Dispose();
		}

		[SetUp]
		public void Setup()
		{
			Console.WriteLine("Wiping MongoDB results database {0}", ServiceConfig.MongodbDatabaseName);
			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });
			repository.Wipe();

			ServiceConfig.RecreateXmlDirectory();
		}

		[Test]
		public void ListFilesForBranch_should_list_all_files()
		{
			// given
			string testFilepath1 = Helpers.GetFullPath(Helpers.GetXmlFilename());
			File.WriteAllText(testFilepath1, @"<?xml version=""1.0"" encoding=""utf-8"" ?><tests/>");

			string testFilepath2 = Helpers.GetFullPath(Helpers.GetXmlFilename());
			File.WriteAllText(testFilepath2, @"<?xml version=""1.0"" encoding=""utf-8"" ?><tests/>");

			TestsClient client = Helpers.CreateTestsClient();

			// when
			IEnumerable<string> files = client.ListFilesForBranch(ServiceConfig.BranchName);

			// then
			Assert.That(files, Is.Not.Null);
			Assert.That(files.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetTest_should_return_expected_test_using_position()
		{
			// given
			int testIndex = 1;
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);
			Test expectedTest = testFile.Tests.ToList()[testIndex];
			
			// when
			Test actualTest = client.GetTest(testFile.Filename, ServiceConfig.BranchName, testIndex);

			// then
			Assert.That(actualTest, Is.Not.Null);
			Assert.That(actualTest.Filename, Is.EqualTo(expectedTest.Filename));
			Assert.That(actualTest.ErrorMessage, Is.EqualTo(expectedTest.ErrorMessage));
			Assert.That(actualTest.ShortDescription, Is.EqualTo(expectedTest.ShortDescription));
		}

		[Test]
		public void GetTestFile_should_return_expected_testfile()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);

			// when
			TestFile actualTestFile = client.GetTestFile(testFile.Filename, ServiceConfig.BranchName);

			// then
			Assert.That(actualTestFile, Is.Not.Null);
			Assert.That(actualTestFile.Filename, Is.EqualTo(testFile.Filename));
			Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetXml_should_return_expected_source()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);

			// when
			string xml = client.GetXml(testFile.Filename, ServiceConfig.BranchName);

			// then
			Assert.That(xml, Is.Not.Null);
			Assert.That(xml, Is.StringContaining(@"<?xml version=""1.0"""));
			Assert.That(xml, Is.StringContaining("<tests"));
			Assert.That(xml, Is.StringContaining("<test"));
		}

		[Test]
		public void EditTest_should_save_changes_to_test()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);
			Test expectedTest = testFile.Tests.FirstOrDefault();
			expectedTest.ShortDescription = "new description";

			// when
			bool success = client.EditTest(expectedTest, ServiceConfig.BranchName);

			// then
			Test actualTest = client.GetTest(testFile.Filename, ServiceConfig.BranchName, 0);
			
			Assert.True(success);
			Assert.That(actualTest.ShortDescription, Is.StringContaining("new description"));
		}

		[Test]
		public void CreateTest_should_create_test_for_existing_file()
		{
			// given
			string filename = Helpers.GetXmlFilename();
			TestsClient client = Helpers.CreateTestsClient();
			client.CreateTestFile(new TestFile() { Filename = filename }, ServiceConfig.BranchName);

			var test = new Test()
			{
				Filename = filename,
				Assertions = new List<Assertion>(),
				AvailableVariables = new List<Variable>(),
				CapturedVariables = new List<CapturedVariable>(),
				ErrorMessage = "my error message",
				Headers = new List<HeaderItem>(),
				LongDescription = "desc",
				Method = "POST",
				Url = "url"
			};

			// when
			bool success = client.CreateTest(test, ServiceConfig.BranchName);

			// then
			string fullPath = Helpers.GetFullPath(filename);

			Assert.True(success);
			Assert.True(File.Exists(fullPath));
			Assert.That(new FileInfo(fullPath).Length, Is.GreaterThan(0));
			Assert.That(File.ReadAllText(fullPath), Is.StringContaining(@"errormessage=""my error message"""));
		}

		[Test]
		public void DeleteTest_should_save_changes_to_test()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile expectedTestFile = Helpers.CreateTestFileAndTest(client);

			// when
			bool success = client.DeleteTest(0, expectedTestFile.Filename, ServiceConfig.BranchName);

			// then
			TestFile actualTestFile = client.GetTestFile(expectedTestFile.Filename, ServiceConfig.BranchName);

			Assert.True(success);
			Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CreateTestFile_should_write_file()
		{
			// given
			string filename = Helpers.GetXmlFilename();
			TestsClient client = Helpers.CreateTestsClient();

			// when
			bool success = client.CreateTestFile(new TestFile() { Filename = filename }, ServiceConfig.BranchName);

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
			bool success = client.UpdateTestVariables(testFile, ServiceConfig.BranchName);

			// then
			Assert.True(success);

			TestFile actualTestFile = client.GetTestFile(testFile.Filename, ServiceConfig.BranchName);
			Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetSummariesForToday_should_only_return_todays_results()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);

			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });

			var yesterdayResult = new TestFileResult()
			{
				StartTime = DateTime.Now.AddDays(-1),
				EndTime = DateTime.Now.AddDays(-1).AddSeconds(1),
				Filename = testFile.Filename
			};
			var todayResult1 = new TestFileResult()
			{
				StartTime = DateTime.Now,
				EndTime = DateTime.Now.AddSeconds(1),
				Filename = testFile.Filename
			};
			var todayResult2 = new TestFileResult()
			{
				StartTime = DateTime.Now,
				EndTime = DateTime.Now.AddSeconds(1),
				Filename = testFile.Filename
			};

			repository.AddAsync(yesterdayResult).Wait();
			repository.AddAsync(todayResult1).Wait();
			repository.AddAsync(todayResult2).Wait();

			// when
			IEnumerable<TestFileResultSummary> results = client.GetSummariesForToday();

			// then
			Assert.That(results.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetSummaries_should_return_all_results()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);

			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });

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

			repository.AddAsync(result1).Wait();
			repository.AddAsync(result2).Wait();

			// when
			IEnumerable<TestFileResultSummary> results = client.GetSummaries();

			// then
			Assert.That(results.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetResultById_should_return_expected_result()
		{
			// given
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);

			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });

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

			repository.AddAsync(result1).Wait();
			repository.AddAsync(result2).Wait();

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

			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });

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

			repository.AddAsync(result1).Wait();
			repository.AddAsync(result2).Wait();

			// when
			client.DeleteResultAsync(result2.Id).Wait();

			// then
			TestFileResult deletedResult = client.GetResultById(result2.Id);
			Assert.That(deletedResult, Is.Null);

			TestFileResult otherResult = client.GetResultById(result1.Id);
			Assert.That(otherResult, Is.Not.Null);
		}

		[Test]
		public void DeleteFile_should_delete_file_from_disk()
		{
			// given
			string filename = Helpers.GetXmlFilename();
			TestsClient client = Helpers.CreateTestsClient();
			TestFile testFile = Helpers.CreateTestFileAndTest(client);

			// when
			bool success = client.DeleteFile(testFile.Filename, ServiceConfig.BranchName);

			// then
			string fullPath = Helpers.GetFullPath(filename);

			Assert.True(success);
			Assert.False(File.Exists(fullPath));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Syringe.Client.Http;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Client
{
	public class TestsClient : ITestService
	{
		internal readonly string ServiceUrl;
		private readonly FlurlWrapper _testFileWrapper;
		private readonly FlurlWrapper _testWrapper;

		public TestsClient(string serviceUrl)
		{
			ServiceUrl = serviceUrl;
			var factory = new CustomHttpClientFactory(serviceUrl);

			_testFileWrapper = new FlurlWrapper(factory, "/api/testfile");
			_testWrapper = new FlurlWrapper(factory, "/api/test");
		}

		public IEnumerable<string> ListFiles()
		{
			return _testFileWrapper.Get<List<string>>("testfiles").Result;
		}

		public TestFile GetTestFile(string filename)
		{
			_testFileWrapper.AddParameter("filename", filename);
			return _testFileWrapper.Get<TestFile>("").Result;
		}

		public string GetRawFile(string filename)
		{
			_testFileWrapper.AddParameter("filename", filename);
			return _testFileWrapper.Get<string>("raw").Result;
		}

		public bool EditTest(string filename, int position, Test test)
		{
			_testFileWrapper.AddParameter("filename", filename);
			_testFileWrapper.AddParameter("position", Convert.ToString(position));
			return _testFileWrapper.Patch<bool>("", test).Result;
		}

		public bool CreateTest(string filename, Test test)
		{
			_testWrapper.AddParameter("filename", filename);
			return _testWrapper.Post<bool>("", test).Result;
		}

		public bool DeleteTest(int position, string filename)
		{
			_testWrapper.AddParameter("filename", filename);
			_testWrapper.AddParameter("position", Convert.ToString(position));
			return _testWrapper.Delete<bool>("").Result;
		}

		public bool CopyTest(int position, string filename)
		{
			_testWrapper.AddParameter("filename", filename);
			_testWrapper.AddParameter("position", Convert.ToString(position));
			return _testWrapper.Post<bool>("copy", "").Result;
		}

		public bool CreateTestFile(TestFile testFile)
		{
			_testFileWrapper.AddParameter("filename", testFile.Filename);
			return _testFileWrapper.Post<bool>("copy", testFile).Result;
		}

		public bool CopyTestFile(string sourceFileName, string targetFileName)
		{
			_testFileWrapper.AddParameter("sourceFileName", sourceFileName);
			_testFileWrapper.AddParameter("targetFileName", targetFileName);

			return _testFileWrapper.Post<bool>("copy", "").Result;
		}

		public bool UpdateTestVariables(TestFile testFile)
		{
			return _testFileWrapper.Post<bool>("variables", testFile).Result;
		}

		public async Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "")
		{
			_testWrapper.AddParameter("filename", pageNumber.ToString());
			_testWrapper.AddParameter("noOfResults", noOfResults.ToString());
			_testWrapper.AddParameter("fromDateTime", fromDateTime.ToString(CultureInfo.InvariantCulture));
			_testWrapper.AddParameter("environment", environment);

			return await _testWrapper.Get<TestFileResultSummaryCollection>("results");
		}

		public async Task<TestFileResult> GetResultById(Guid id)
		{
			_testWrapper.AddParameter("id", id.ToString());
			return await _testWrapper.Get<TestFileResult>("result");
		}

		public bool DeleteResult(Guid id)
		{
			_testWrapper.AddParameter("id", id.ToString());
			return _testWrapper.Delete<bool>("result").Result;
		}

		public bool DeleteFile(string fileName)
		{
			_testFileWrapper.AddParameter("filename", fileName);
			return _testFileWrapper.Delete<bool>("result").Result;
		}
	}
}
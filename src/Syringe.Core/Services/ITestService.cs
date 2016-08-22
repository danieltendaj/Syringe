using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Services
{
	public interface ITestService
	{
		IEnumerable<string> ListFiles();
		TestFile GetTestFile(string filename);
	    string GetRawFile(string filename);
        bool EditTest(string filename, int position, Test test);
	    bool CreateTest(string filename, Test test);
        bool DeleteTest(int position, string fileName);
        bool CopyTest(int position, string fileName);
        bool DeleteFile(string fileName);
	    bool CreateTestFile(TestFile testFile);
	    bool CopyTestFile(string sourceFileName, string targetFileName);
        bool UpdateTestVariables(TestFile testFile);
        Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "");
        TestFileResult GetResultById(Guid id);
        bool DeleteResult(Guid id);
	}
}
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
		Test GetTest(string filename, string branchName, int position);
		TestFile GetTestFile(string filename, string branchName);
	    string GetXml(string filename, string branchName);
        bool EditTest(Test test, string branchName);
	    bool CreateTest(Test test, string branchName);
        bool DeleteTest(int position, string fileName, string branchName);
	    bool CreateTestFile(TestFile testFile, string branchName);
	    bool UpdateTestVariables(TestFile testFile, string branchName);
	    IEnumerable<TestFileResultSummary> GetSummariesForToday();
	    IEnumerable<TestFileResultSummary> GetSummaries();
        TestFileResult GetResultById(Guid id);
        Task DeleteResultAsync(Guid id);
	    bool DeleteFile(string fileName, string branchName);
	}
}
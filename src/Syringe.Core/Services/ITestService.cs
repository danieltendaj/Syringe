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
		Test GetTest(string filename, int position);
		TestFile GetTestFile(string filename);
	    string GetXml(string filename);
        bool EditTest(Test test);
	    bool CreateTest(Test test);
        bool DeleteTest(int position, string fileName);
        bool CopyTest(int position, string fileName);
        bool DeleteFile(string fileName);
	    bool CreateTestFile(TestFile testFile);
	    bool UpdateTestVariables(TestFile testFile);
        Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20);
        TestFileResult GetResultById(Guid id);
        Task DeleteResultAsync(Guid id);
	}
}
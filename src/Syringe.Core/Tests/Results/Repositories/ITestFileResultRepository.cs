using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syringe.Core.Tests.Results.Repositories
{
    public interface ITestFileResultRepository : IDisposable
	{
		Task Add(TestFileResult testFileResult);
        Task Delete(Guid testFileResultId);
        Task DeleteBeforeDate(DateTime date);
        //TODO: Make Async to follow pattern
        TestFileResult GetById(Guid id);
		Task Wipe();
        Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "");
	}
}
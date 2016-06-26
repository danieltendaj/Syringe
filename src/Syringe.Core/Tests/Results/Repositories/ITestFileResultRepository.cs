using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syringe.Core.Tests.Results.Repositories
{
	public interface ITestFileResultRepository : IDisposable
	{
		Task AddAsync(TestFileResult testFileResult);
		Task DeleteAsync(Guid testFileResultId);
		TestFileResult GetById(Guid id);
		void Wipe();
        Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "");
	}
}
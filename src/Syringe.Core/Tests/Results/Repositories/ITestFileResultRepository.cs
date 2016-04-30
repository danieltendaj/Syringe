using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syringe.Core.Tests.Results.Repositories
{
	public interface ITestFileResultRepository
	{
		Task AddAsync(TestFileResult testFileResult);
		Task DeleteAsync(Guid testFileResultId);
		TestFileResult GetById(Guid id);
		void Wipe();
        TestFileResultSummaryCollection GetSummaries(int pageNumber = 1, int noOfResults = 20);
        TestFileResultSummaryCollection GetSummariesForToday(int pageNumber = 1, int noOfResults = 20);
	}
}
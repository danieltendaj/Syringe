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
		IEnumerable<TestFileResultSummary> GetSummaries();
		IEnumerable<TestFileResultSummary> GetSummariesForToday();
	}
}
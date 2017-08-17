using System;
using System.Threading.Tasks;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestFileResultRepository : IDisposable
	{
		Task Add(TestFileResult testFileResult);

		Task Delete(Guid testFileResultId);

		Task DeleteBeforeDate(DateTime date);

		Task<TestFileResult> GetById(Guid id);

		Task ClearDatabase();

		Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "");
	}
}
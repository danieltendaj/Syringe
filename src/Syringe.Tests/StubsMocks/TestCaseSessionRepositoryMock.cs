using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Tests.StubsMocks
{
	internal class TestFileResultRepositoryMock : ITestFileResultRepository
	{
		public TestFileResult SavedSession { get; set; }

		public Task DeleteAsync(Guid testFileResultId)
		{
			return Task.FromResult<object>(null);
		}

		public TestFileResult GetById(Guid id)
		{
			return new TestFileResult();
		}

		public void Wipe()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TestFileResultSummary> GetSummaries()
		{
			return new List<TestFileResultSummary>();
		}

		public IEnumerable<TestFileResultSummary> GetSummariesForToday()
		{
			return new List<TestFileResultSummary>();
		}

		public Task AddAsync(TestFileResult testFileResult)
		{
			SavedSession = testFileResult;
		    return Task.FromResult<object>(null);
		}
	}
}
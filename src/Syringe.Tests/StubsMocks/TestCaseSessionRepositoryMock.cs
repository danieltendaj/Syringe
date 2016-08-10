using System;
using System.Threading.Tasks;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Tests.StubsMocks
{
    internal class TestFileResultRepositoryMock : ITestFileResultRepository
    {
        public TestFileResult SavedTestFileResult { get; set; }
        public bool Disposed { get; set; }

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
        }

        public Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            return Task.FromResult(new TestFileResultSummaryCollection());
        }

        public Task AddAsync(TestFileResult testFileResult)
        {
            SavedTestFileResult = testFileResult;
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
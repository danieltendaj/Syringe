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

        public Task Delete(Guid testFileResultId)
        {
            return Task.FromResult<object>(null);
        }

        public Task DeleteBeforeDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public TestFileResult GetById(Guid id)
        {
            return new TestFileResult();
        }

        Task ITestFileResultRepository.Wipe()
        {
            throw new NotImplementedException();
        }

        public Task<TestFileResultSummaryCollection> GetSummaries(DateTime fromDateTime, int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            return Task.FromResult(new TestFileResultSummaryCollection());
        }

        public Task Add(TestFileResult testFileResult)
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
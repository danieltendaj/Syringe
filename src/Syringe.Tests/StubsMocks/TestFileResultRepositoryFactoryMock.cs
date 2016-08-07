using Syringe.Core.Tests.Results.Repositories;

namespace Syringe.Tests.StubsMocks
{
    internal class TestFileResultRepositoryFactoryMock : ITestFileResultRepositoryFactory
    {
        public TestFileResultRepositoryMock Repository { get; set; }

        public TestFileResultRepositoryFactoryMock()
        {
            Repository = new TestFileResultRepositoryMock();
        }

        public ITestFileResultRepository GetRepository()
        {
            return Repository;
        }
    }
}
using StructureMap;

namespace Syringe.Core.Tests.Results.Repositories
{
    public class TestFileResultRepositoryFactory : ITestFileResultRepositoryFactory
    {
        internal readonly IContext Context;

        public TestFileResultRepositoryFactory(IContext context)
        {
            Context = context;
        }

        public ITestFileResultRepository GetRepository()
        {
            return Context.GetInstance<ITestFileResultRepository>();
        }
    }
}
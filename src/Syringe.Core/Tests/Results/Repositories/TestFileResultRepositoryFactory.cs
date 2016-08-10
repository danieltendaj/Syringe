using StructureMap;

namespace Syringe.Core.Tests.Results.Repositories
{
    public class TestFileResultRepositoryFactory : ITestFileResultRepositoryFactory
    {
        private readonly IContext _context;

        public TestFileResultRepositoryFactory(IContext context)
        {
            _context = context;
        }

        public ITestFileResultRepository GetRepository()
        {
            return _context.GetInstance<ITestFileResultRepository>();
        }
    }
}
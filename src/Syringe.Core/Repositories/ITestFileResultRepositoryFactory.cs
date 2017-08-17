namespace Syringe.Core.Repositories
{
    public interface ITestFileResultRepositoryFactory
    {
        ITestFileResultRepository GetRepository();
    }
}
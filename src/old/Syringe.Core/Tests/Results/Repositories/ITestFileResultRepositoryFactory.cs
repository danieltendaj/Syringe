namespace Syringe.Core.Tests.Results.Repositories
{
    public interface ITestFileResultRepositoryFactory
    {
        ITestFileResultRepository GetRepository();
    }
}
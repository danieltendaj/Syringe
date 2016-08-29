using Syringe.Core.Tests;

namespace Syringe.Service.Parallel
{
    public interface ITestFileAssembler
    {
        TestFile AssembleTestFile(string testFileName, string environment);
    }
}
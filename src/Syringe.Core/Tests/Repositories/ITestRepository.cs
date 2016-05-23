using System.Collections.Generic;

namespace Syringe.Core.Tests.Repositories
{
    public interface ITestRepository
    {
        IEnumerable<string> ListFiles();
        TestFile GetTestFile(string filename);
        Test GetTest(string filename, int position);
        bool SaveTest(Test test);
        bool SaveTest(string filename, int position, Test test);
        bool CreateTest(Test test);
        bool DeleteTest(int position, string filename);
        bool CreateTestFile(TestFile testFile);
        bool UpdateTestVariables(TestFile testFile);
        string GetRawFile(string filename);
        bool DeleteFile(string filename);
    }
}
using System;
using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Core.Repositories
{
    public interface ITestRepository
    {
        IEnumerable<string> ListFiles();
        TestFile GetTestFile(string filename, string branchName);
        Test GetTest(string filename, int position);
        bool SaveTest(Test test, string branchName);
        bool CreateTest(Test test, string branchName);
        bool DeleteTest(int position, string fileName, string branchName);
        bool CreateTestFile(TestFile testFile, string branchName);
        bool UpdateTestVariables(TestFile testFile, string branchName);
        string GetXml(string filename, string branchName);
        bool DeleteFile(string fileName, string branchName);
    }
}
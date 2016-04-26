using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Core.Repositories.Json
{
    public class TestRepository : ITestRepository
    {


        public IEnumerable<string> ListFiles()
        {
            throw new System.NotImplementedException();
        }

        public TestFile GetTestFile(string filename)
        {
            throw new System.NotImplementedException();
        }

        public Test GetTest(string filename, int position)
        {
            throw new System.NotImplementedException();
        }

        public bool SaveTest(Test test)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateTest(Test test)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteTest(int position, string fileName)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateTestFile(TestFile testFile)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateTestVariables(TestFile testFile)
        {
            throw new System.NotImplementedException();
        }

        public string GetXml(string filename)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteFile(string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}
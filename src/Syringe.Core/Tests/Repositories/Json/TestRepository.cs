using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.IO;

namespace Syringe.Core.Tests.Repositories.Json
{
    public class TestRepository : ITestRepository
    {
        private readonly ITestFileReader _testFileReader;
        private readonly ITestFileWriter _testFileWriter;
        private readonly IFileHandler _fileHandler;

        public TestRepository(ITestFileReader testFileReader, ITestFileWriter testFileWriter, IFileHandler fileHandler)
        {
            _testFileReader = testFileReader;
            _testFileWriter = testFileWriter;
            _fileHandler = fileHandler;
        }

        public IEnumerable<string> ListFiles()
        {
            return _fileHandler.GetFileNames();
        }

        public TestFile GetTestFile(string filename)
        {
            string fullPath = _fileHandler.GetFileFullPath(filename);
            string data = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(data))
            {
                TestFile testFile = _testFileReader.Read(stringReader);
                testFile.Filename = filename;

                return testFile;
            }
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
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            string fullPath = _fileHandler.GetFileFullPath(test.Filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            TestFile collection;

            using (var stringReader = new StringReader(xml))
            {
                collection = _testFileReader.Read(stringReader);

                collection.Tests = collection.Tests.Concat(new[] { test });
            }

            string contents = _testFileWriter.Write(collection);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool DeleteTest(int position, string fileName)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateTestFile(TestFile testFile)
        {
            testFile.Filename = _fileHandler.CreateFilename(testFile.Filename);

            string filePath = _fileHandler.CreateFileFullPath(testFile.Filename);
            bool fileExists = _fileHandler.FileExists(filePath);

            if (fileExists)
            {
                throw new IOException("File already exists");
            }

            string contents = _testFileWriter.Write(testFile);

            return _fileHandler.WriteAllText(filePath, contents);
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using Syringe.Core.IO;

namespace Syringe.Core.Tests.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly ITestFileReader _testFileReader;
        private readonly ITestFileWriter _testFileWriter;
        private readonly IFileHandler _fileHandler;
        private readonly IConfiguration _configuration;

        public TestRepository(ITestFileReader testFileReader, ITestFileWriter testFileWriter, IFileHandler fileHandler, IConfiguration configuration)
        {
            _testFileReader = testFileReader;
            _testFileWriter = testFileWriter;
            _fileHandler = fileHandler;
            _configuration = configuration;
        }

        public Test GetTest(string filename, int position)
        {
            string fullPath = _fileHandler.GetFileFullPath(filename);
            string fileContents = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(fileContents))
            {
                TestFile testFile = _testFileReader.Read(stringReader);
                Test test = testFile.Tests.ElementAtOrDefault(position);

                if (test == null)
                {
                    throw new NullReferenceException("Could not find specified Test Case:" + position);
                }

                return test;
            }
        }

        public bool CreateTest(string filename, Test test)
        {
            string fullPath = _fileHandler.GetFileFullPath(filename);
            string fileContents = _fileHandler.ReadAllText(fullPath);

            TestFile testFile;

            using (var stringReader = new StringReader(fileContents))
            {
                testFile = _testFileReader.Read(stringReader);

                testFile.Tests = testFile.Tests.Concat(new[] { test });
            }

            return SaveTestFile(testFile, fullPath);
        }

        public bool SaveTest(string filename, int position, Test test)
        {
            string fullPath = _fileHandler.GetFileFullPath(filename);
            string fileContents = _fileHandler.ReadAllText(fullPath);

            TestFile testFile;

            using (var stringReader = new StringReader(fileContents))
            {
                testFile = _testFileReader.Read(stringReader);
            }

            Test singleTest = testFile.Tests.ElementAt(position);

            singleTest.Description = test.Description;
            singleTest.Headers = test.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList();
            singleTest.Method = test.Method;
            singleTest.CapturedVariables = test.CapturedVariables;
            singleTest.PostBody = test.PostBody;
            singleTest.Assertions = test.Assertions;
            singleTest.Description = test.Description;
            singleTest.Url = test.Url;
            singleTest.ExpectedHttpStatusCode = test.ExpectedHttpStatusCode;
            singleTest.ScriptSnippets = test.ScriptSnippets;
            singleTest.TestConditions = test.TestConditions;

            return SaveTestFile(testFile, fullPath);
        }

        public bool DeleteTest(int position, string filename)
        {
            string fullPath = _fileHandler.GetFileFullPath(filename);
            string fileContents = _fileHandler.ReadAllText(fullPath);

            TestFile testFile;

            using (var stringReader = new StringReader(fileContents))
            {
                testFile = _testFileReader.Read(stringReader);

                Test testToDelete = testFile.Tests.ElementAtOrDefault(position);

                if (testToDelete == null)
                {
                    throw new NullReferenceException(string.Concat("could not find test case:", position));
                }

                testFile.Tests = testFile.Tests.Where(x => x != testToDelete);
            }

            return SaveTestFile(testFile, fullPath);
        }

        public bool CreateTestFile(TestFile testFile)
        {
            testFile.Filename = _fileHandler.GetFilenameWithExtension(testFile.Filename);

            string fullPath = _fileHandler.CreateFileFullPath(testFile.Filename);
            bool fileExists = _fileHandler.FileExists(fullPath);

            if (fileExists)
            {
                throw new IOException("File already exists");
            }

            return SaveTestFile(testFile, fullPath);
        }

        public bool UpdateTestVariables(TestFile testFile)
        {
            string fileFullPath = _fileHandler.GetFileFullPath(testFile.Filename);
            string fileContents = _fileHandler.ReadAllText(fileFullPath);

            using (var stringReader = new StringReader(fileContents))
            {
                TestFile updatedTestFile = _testFileReader.Read(stringReader);

                updatedTestFile.Variables = testFile.Variables;

                return SaveTestFile(updatedTestFile, fileFullPath);
            }
        }

        public bool UpdateTests(TestFile testFile)
        {
            string fileFullPath = _fileHandler.GetFileFullPath(testFile.Filename);
            string fileContents = _fileHandler.ReadAllText(fileFullPath);

            using (var stringReader = new StringReader(fileContents))
            {
                TestFile updatedTestFile = _testFileReader.Read(stringReader);
                updatedTestFile.Tests = testFile.Tests;

                return SaveTestFile(updatedTestFile, fileFullPath);
            }
        }

        private bool SaveTestFile(TestFile testFile, string fileFullPath)
        {
            if (testFile.EngineVersion > _configuration.EngineVersion)
            {
                throw new InvalidEngineException("The file you are trying to save was built with a newer version of Syringe. Please update your copy of Syringe.");
            }

            testFile.EngineVersion = _configuration.EngineVersion;

            string contents = _testFileWriter.Write(testFile);
            return _fileHandler.WriteAllText(fileFullPath, contents);
        }

        public TestFile GetTestFile(string filename)
        {
            string fullPath = _fileHandler.GetFileFullPath(filename);
            string fileContents = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(fileContents))
            {
                TestFile testFile = _testFileReader.Read(stringReader);
                testFile.Filename = filename;

                return testFile;
            }
        }

        public string GetRawFile(string filename)
        {
            var fullPath = _fileHandler.GetFileFullPath(filename);
            return _fileHandler.ReadAllText(fullPath);
        }

        public bool DeleteFile(string filename)
        {
            var fullPath = _fileHandler.GetFileFullPath(filename);
            return _fileHandler.DeleteFile(fullPath);
        }

        public IEnumerable<string> ListFiles()
        {
            return _fileHandler.GetFileNames();
        }
    }
}
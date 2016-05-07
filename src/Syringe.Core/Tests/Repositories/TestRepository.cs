using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.IO;

namespace Syringe.Core.Tests.Repositories
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

		public Test GetTest(string filename, int position)
		{
			string fullPath = _fileHandler.GetFileFullPath(filename);
			string xml = _fileHandler.ReadAllText(fullPath);

			using (var stringReader = new StringReader(xml))
			{
				TestFile testFile = _testFileReader.Read(stringReader);
				Test test = testFile.Tests.ElementAtOrDefault(position);

				if (test == null)
				{
					throw new NullReferenceException("Could not find specified Test Case:" + position);
				}

				test.Filename = filename;

				return test;
			}
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

		public bool SaveTest(Test test)
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

				Test singleTest = collection.Tests.ElementAt(test.Position);

				singleTest.Description = test.Description;
			    singleTest.Headers = test.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList();
				singleTest.Method = test.Method;
				singleTest.Filename = test.Filename;
				singleTest.CapturedVariables = test.CapturedVariables;
				singleTest.PostBody = test.PostBody;
				singleTest.Assertions = test.Assertions;
				singleTest.Description = test.Description;
				singleTest.Url = test.Url;
				singleTest.ExpectedHttpStatusCode = test.ExpectedHttpStatusCode;
				singleTest.BeforeExecuteScript = test.BeforeExecuteScript;
			}

			string contents = _testFileWriter.Write(collection);

			return _fileHandler.WriteAllText(fullPath, contents);
		}

		public bool DeleteTest(int position, string fileName)
		{
			string fullPath = _fileHandler.GetFileFullPath(fileName);
			string xml = _fileHandler.ReadAllText(fullPath);

			TestFile testFile;

			using (var stringReader = new StringReader(xml))
			{
				testFile = _testFileReader.Read(stringReader);

				Test testToDelete = testFile.Tests.ElementAtOrDefault(position);

				if (testToDelete == null)
				{
					throw new NullReferenceException(string.Concat("could not find test case:", position));
				}

				testFile.Tests = testFile.Tests.Where(x => x != testToDelete);
			}

			string contents = _testFileWriter.Write(testFile);

			return _fileHandler.WriteAllText(fullPath, contents);
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
			string fileFullPath = _fileHandler.GetFileFullPath(testFile.Filename);
			string xml = _fileHandler.ReadAllText(fileFullPath);

			using (var stringReader = new StringReader(xml))
			{
				TestFile updatedTestFile = _testFileReader.Read(stringReader);

				updatedTestFile.Variables = testFile.Variables;

				string contents = _testFileWriter.Write(updatedTestFile);
				return _fileHandler.WriteAllText(fileFullPath, contents);
			}
		}

		public TestFile GetTestFile(string filename)
		{
			string fullPath = _fileHandler.GetFileFullPath(filename);
			string xml = _fileHandler.ReadAllText(fullPath);

			using (var stringReader = new StringReader(xml))
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

		public bool DeleteFile(string fileName)
		{
			var fullPath = _fileHandler.GetFileFullPath(fileName);
			return _fileHandler.DeleteFile(fullPath);
		}

	    public bool CopyTest(int position, string fileName)
	    {
	        throw new NotImplementedException();
	    }

	    public IEnumerable<string> ListFiles()
		{
			return _fileHandler.GetFileNames();
		}
	}
}
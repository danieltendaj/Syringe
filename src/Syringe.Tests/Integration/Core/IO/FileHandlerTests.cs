using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.IO;

namespace Syringe.Tests.Integration.Core.IO
{
	public class FileHandlerTests
    {
	    private JsonConfiguration _jsonConfiguration;
	    private FileHandler _fileHandler;
	    private string _baseDirectory;

        [TestFixtureSetUp]
	    public void TestfixtureSetup()
        {
            _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "integration", "FileHandlerTests");
            CreateTestFileDirectory();
        }

	    private void CreateTestFileDirectory()
	    {
	        if (Directory.Exists(_baseDirectory))
	        {
	            Directory.Delete(_baseDirectory, true);
	        }

	        Directory.CreateDirectory(_baseDirectory);
	    }

	    [SetUp]
	    public void Setup()
		{
	        _jsonConfiguration = new JsonConfiguration();
	        _jsonConfiguration.TestFilesBaseDirectory = _baseDirectory;

	        _fileHandler = new FileHandler(_jsonConfiguration);
		}

        [Test]
        public void DeleteFile_should_remove_file_from_disk()
        {
            // given
            string path = Path.Combine(_baseDirectory, "test-DeleteFile.json");
            File.WriteAllText(path, "{}");

            // when
            bool actualResult = _fileHandler.DeleteFile(path);

            // then
            Assert.That(actualResult, Is.EqualTo(true));
            Assert.That(File.Exists(path), Is.False);
        }

        [Test]
        public void FileExists_should_return_true_when_file_exists()
        {
            // given
            string path = Path.Combine(_baseDirectory, "test-FileExists.json");
            File.WriteAllText(path, "{}");

            // when
            bool actualResult = _fileHandler.FileExists(path);

            // then
            Assert.That(actualResult, Is.EqualTo(true));
        }

        [Test]
        public void WriteAllText_should_write_text_to_the_path()
        {
            // given
            string path = Path.Combine(_baseDirectory, "test-WriteAllText.json");

            // when
            bool actualResult = _fileHandler.WriteAllText(path, "{ json: 1 }");

            // then
            Assert.That(actualResult, Is.EqualTo(true));
            Assert.That(File.Exists(path), Is.True);
            Assert.That(File.ReadAllText(path), Is.EqualTo("{ json: 1 }"));
        }

        [Test]
        public void ReadAllText_should_write_text_to_the_path()
        {
            // given
            string path = Path.Combine(_baseDirectory, "test-ReadAllText.json");
            File.WriteAllText(path, "{ json: 1 }");

            // when
            string actualText = _fileHandler.ReadAllText(path);

            // then
            Assert.That(actualText, Is.EqualTo("{ json: 1 }"));
        }

        [Test]
        public void GetFileNames_should_write_text_to_the_path()
        {
            // given
            CreateTestFileDirectory();
            string path1 = Path.Combine(_baseDirectory, "test-GetFileNames1.json");
            File.WriteAllText(path1, "{ json: 1 }");

            string path2 = Path.Combine(_baseDirectory, "test-GetFileNames2.json");
            File.WriteAllText(path2, "{ json: 2 }");

            // when
            IEnumerable<string> files = _fileHandler.GetFileNames();

            // then
            Assert.That(files.Count(), Is.EqualTo(2));
        }

        [Test]
        public void CreateFileFullPath_should_combine_testdirectory_with_filename()
        {
            // given
            string expectedPath = Path.Combine(_baseDirectory, "test-GetFileNames1.json");

            // when
            string actualFullPath = _fileHandler.CreateFileFullPath("test-GetFileNames1.json");

            // then
            Assert.That(actualFullPath, Is.EqualTo(expectedPath));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetFilenameWithExtension_should_throw_argument_null_exception_when_filename_is_empty(string fileName)
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _fileHandler.GetFilenameWithExtension(fileName));
        }

        [TestCase("test")]
        [TestCase("cases")]
        public void GetFilenameWithExtension_should_add_file_extension_if_it_is_missing(string fileName)
        {
            // given
            string expectedFileExtension = "json";
            string expectedFileName = $"{fileName}.{expectedFileExtension}";

            // when
            string createdFileName = _fileHandler.GetFilenameWithExtension(fileName);

            // then
            Assert.That(createdFileName, Is.EqualTo(expectedFileName));
        }

        [TestCase("test")]
        [TestCase("cases")]
        public void GetFilenameWithExtension_should__return_correct_name_if_passed_in_correctly(string fileName)
        {
            // given
            string expectedFileExtension = "json";
            fileName += $".{expectedFileExtension}";

            // when
            string createdFileName = _fileHandler.GetFilenameWithExtension(fileName);

            // then
            Assert.That(createdFileName, Is.EqualTo(fileName));
        }
    }
}
using System;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.IO;

namespace Syringe.Tests.Unit.Core.IO
{
    [TestFixture(TestFileFormat.Json)]
    public class FileHandlerTests
    {
        private readonly TestFileFormat _testFileFormat;
        private Mock<IConfiguration> _configurationMock;

        private TestFileInfo _testsFileInfo;
        private string _testsDirectory;

        public FileHandlerTests(TestFileFormat testFileFormat)
        {
            _testFileFormat = testFileFormat;
        }

        [TearDown]
        public void TearDownFixture()
        {
            Directory.Delete(_testsDirectory, true);
        }

        [SetUp]
        public void Setup()
        {
            _testsDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "tests");

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x.TestFilesBaseDirectory).Returns(_testsDirectory);
            _configurationMock.SetupGet(x => x.TestFileFormat).Returns(_testFileFormat);

            PrepareTestDataOnDisk();
        }

        private void PrepareTestDataOnDisk()
        {
            string extension = _testFileFormat.ToString();

            _testsFileInfo = new TestFileInfo
            {
                Filename = $"test.{extension}",
                ReadFullPath = Path.Combine(_testsDirectory, $"test.{extension}"),
                WriteFullPath = Path.Combine(_testsDirectory, $"testsWrite.{extension}"),
                DeleteFullPath = Path.Combine(_testsDirectory, $"fileToDelete.{extension}")
            };

            if (!Directory.Exists(_testsDirectory))
            {
                Directory.CreateDirectory(_testsDirectory);
            }

            if (!File.Exists(_testsFileInfo.ReadFullPath))
            {
                File.WriteAllText(_testsFileInfo.ReadFullPath, "Test Data");
            }

            if (!File.Exists(_testsFileInfo.DeleteFullPath))
            {
                File.WriteAllText(_testsFileInfo.DeleteFullPath, "Delete file");
            }

            if (File.Exists(_testsFileInfo.WriteFullPath))
            {
                File.Delete(_testsFileInfo.WriteFullPath);
            }
        }

        [Test]
        public void GetFileFullPath_should_throw_FileNotFoundException_if_file_is_missing()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);
            string fileName = "filedoesnotexist.json";

            // when + then
            Assert.Throws<FileNotFoundException>(() => fileHandler.GetFileFullPath(fileName));
        }

        [Test]
        public void GetFileFullPath_should_return_file_path_if_file_exists()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            string fileFullPath = fileHandler.GetFileFullPath(_testsFileInfo.Filename);

            // then
            Assert.That(fileFullPath, Is.EqualTo(_testsFileInfo.ReadFullPath));
        }

        [Test]
        public void FileExists_should_return_true_if_file_exists()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            bool fileExists = fileHandler.FileExists(_testsFileInfo.ReadFullPath);

            // then
            Assert.That(fileExists, Is.True);
        }

        [Test]
        public void FileExists_should_return_false_if_does_not_exist()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            bool fileExists = fileHandler.FileExists("somefakepath/filedoesnotexist.json");

            // then
            Assert.That(fileExists, Is.False);
        }

        [Test]
        public void ReadAllText_should_return_file_contents()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            string allText = fileHandler.ReadAllText(_testsFileInfo.ReadFullPath);

            // then
            Assert.IsTrue(allText.Contains("Test Data"));
        }

        [Test]
        public void WriteAllText_should_return_true_when_contents_written()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            bool allText = fileHandler.WriteAllText(_testsFileInfo.WriteFullPath, "test");

            // then
            Assert.That(allText, Is.True);
        }

        [Test]
        public void WriteAllText_should_write_expected_text_to_file()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);
            string expectedValue = "I AM WHAT YOU SHOULD EXPECTED " + _testFileFormat;

            // when
            fileHandler.WriteAllText(_testsFileInfo.WriteFullPath, expectedValue);

            // then
            string writtenValue = File.ReadAllText(_testsFileInfo.WriteFullPath);
            Assert.That(writtenValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void GetFileNames_should_get_filenames_list()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            string[] files = fileHandler.GetFileNames().ToArray();

            // then
            Assert.That(files.Length, Is.EqualTo(2));

            bool testFileDetected = files.Any(x => x.Equals(_testsFileInfo.Filename, StringComparison.InvariantCultureIgnoreCase));
            Assert.That(testFileDetected, Is.True);
        
            bool deleteFileDetected = files.Any(x => x.Equals(Path.GetFileName(_testsFileInfo.DeleteFullPath), StringComparison.InvariantCultureIgnoreCase));
            Assert.That(deleteFileDetected, Is.True);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void CreateFilename_should_throw_argument_null_exception_when_filename_is_empty(string fileName)
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => new FileHandler(_configurationMock.Object).CreateFilename(fileName));
        }

        [TestCase("test")]
        [TestCase("cases")]
        public void CreateFilename_should_add_file_extension_if_it_is_missing(string fileName)
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);
            string expectedFileExtension = _testFileFormat.ToString().ToLower();
            string expectedFileName = $"{fileName}.{expectedFileExtension}";

            // when
            string createdFileName = fileHandler.CreateFilename(fileName);

            // then
            Assert.That(createdFileName, Is.EqualTo(expectedFileName));
        }

        [TestCase("test")]
        [TestCase("cases")]
        public void CreateFilename_should__return_correct_name_if_passed_in_correctly(string fileName)
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);
            string expectedFileExtension = _testFileFormat.ToString().ToLower();
            fileName += $".{expectedFileExtension}";

            // when
            string createdFileName = fileHandler.CreateFilename(fileName);

            // then
            Assert.That(createdFileName, Is.EqualTo(fileName));
        }

        [Test]
        public void WriteAllText_should_return_false_when_text_failed_to_write()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);

            // when
            bool allText = fileHandler.WriteAllText("invalidPath%*()", "test");

            // then
            Assert.That(allText, Is.False);
        }

        [Test]
        public void DeleteFile_should_delete_file_and_return_true_when_file_is_deleted()
        {
            // given
            FileHandler fileHandler = new FileHandler(_configurationMock.Object);
            Assert.That(File.Exists(_testsFileInfo.DeleteFullPath), Is.True);

            // when
            bool allText = fileHandler.DeleteFile(_testsFileInfo.DeleteFullPath);

            // then
            Assert.That(File.Exists(_testsFileInfo.DeleteFullPath), Is.False);
            Assert.That(allText, Is.True);
        }

        private class TestFileInfo
        {
            public string Filename { get; set; }
            public string ReadFullPath { get; set; }
            public string WriteFullPath { get; set; }
            public string DeleteFullPath { get; set; }
        }
    }
}

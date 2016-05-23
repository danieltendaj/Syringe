using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.IO;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;

namespace Syringe.Tests.Unit.Core.Tests.Repositories
{
    [TestFixture]
    public class TestRepositoryTests
    {
        private Mock<ITestFileReader> _testFileReader;
        private Mock<ITestFileWriter> _testFileWriter;
        private Mock<IFileHandler> _fileHandler;
        private TestRepository _testRepository;

        [SetUp]
        public void Setup()
        {
            _testFileReader = new Mock<ITestFileReader>();
            _testFileWriter = new Mock<ITestFileWriter>();
            _fileHandler = new Mock<IFileHandler>();

            _fileHandler.Setup(x => x.GetFileFullPath(It.IsAny<string>())).Returns("path");
            _fileHandler.Setup(x => x.CreateFileFullPath(It.IsAny<string>())).Returns("filepath.xml");
            _fileHandler.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("<xml></xml>");
            _testFileReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new TestFile { Filename = "filepath.xml", Tests = new List<Test> { new Test() } });
            _testRepository = new TestRepository(_testFileReader.Object, _testFileWriter.Object, _fileHandler.Object);
            _fileHandler.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _fileHandler.Setup(x => x.GetFileNames()).Returns(new List<string> { { "test" } });
            _testFileWriter.Setup(x => x.Write(It.IsAny<TestFile>())).Returns("<xml></xml>");
        }

        [Test]
        public void GetTest_should_throw_null_reference_exception_when_position_is_invalid()
        {
            // given + when
            _testFileReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new TestFile());

            // then
            Assert.Throws<NullReferenceException>(() => _testRepository.GetTest(It.IsAny<string>(), It.IsAny<int>()));
        }

        [Test]
        public void GetTest_should_set_parent_filename_when_testfile_is_found()
        {
            // given + when
            var test = _testRepository.GetTest("parentFileName", It.IsAny<int>());

            // then
            Assert.AreEqual("parentFileName", test.Filename);
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveTest_should_throw_null_reference_exception_when_position_is_invalid()
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _testRepository.SaveTest(null));
        }

        [Test]
        public void SaveTest_should_return_true_when_testfile_is_saved()
        {
            // given + when
            bool success = _testRepository.SaveTest(new Test());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void SaveTest_should_return_true_when_test_is_saved()
        {
            // given
            const string filename = "my expected filename.wzzup";
            const int position = 0;

            // when
            bool success = _testRepository.SaveTest(filename, position, new Test());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(filename), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void CreateTest_should_throw_ArgumentNullException_when_test_is_null()
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _testRepository.CreateTest(null));
        }


        [Test]
        public void CreateTest_should_return_true_when_test_is_saved()
        {
            // given + when
            _testFileReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new TestFile());

            bool success = _testRepository.CreateTest(new Test());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void GetTestFile_should_return_testfile()
        {
            // given
            const string expectedFileName = "this is a filename";
            var expectedTest = new TestFile
            {
                Tests = new[]
                {
                    new Test { Position = 0 },
                    new Test { Position = 0 },
                    new Test { Position = 0 },
                }
            };

            _testFileReader
                .Setup(x => x.Read(It.IsAny<StringReader>()))
                .Returns(expectedTest);

            // when
            TestFile testFile = _testRepository.GetTestFile(expectedFileName);

            // then
            Assert.That(testFile, Is.EqualTo(expectedTest));
            Assert.That(testFile.Tests.Count(), Is.EqualTo(3));

            var tests = testFile.Tests.ToArray();
            Assert.That(tests[0].Position, Is.EqualTo(0));
            Assert.That(tests[1].Position, Is.EqualTo(1));
            Assert.That(tests[2].Position, Is.EqualTo(2));
        }

        [Test]
        public void DeleteTest_should_return_true_when_test_exists()
        {
            // given + when
            var success = _testRepository.DeleteTest(It.IsAny<int>(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));
            _testFileReader.Verify(x => x.Read(It.IsAny<TextReader>()), Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void DeleteTest_should_throw_NullReferenceException_when_test_does_not_exist()
        {
            // given + when + then
            Assert.Throws<NullReferenceException>(() => _testRepository.DeleteTest(2, "filePath.xml"));
        }

        [Test]
        public void ListFilesFor_should_return_list_of_file_names()
        {
            // given + when
            IEnumerable<string> filenames = _testRepository.ListFiles();

            // then
            Assert.NotNull(filenames);
            Assert.AreEqual(1, filenames.Count());
            Assert.AreEqual("test", filenames.First());
        }

        [Test]
        public void CreateTestFile_should_throw_IO_exception_if_file_exists()
        {
            // given = when
            _fileHandler.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            // then
            Assert.Throws<IOException>(() => _testRepository.CreateTestFile(new TestFile { Filename = "filePath.xml" }));
        }

        [Test]
        public void CreateTestFile_should_return_true_if_file_does_not_exist()
        {
            // given + when
            _fileHandler.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);
            var testFile = _testRepository.CreateTestFile(new TestFile { Filename = "filePath.xml" });

            // then
            Assert.IsTrue(testFile);
            _fileHandler.Verify(x => x.CreateFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.CreateFilename(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
        }


        [Test]
        public void UpdateTestFile_should_return_true_if_file_exists()
        {
            // given + when
            bool success = _testRepository.UpdateTestVariables(new TestFile { Filename = "filePath.xml" });

            // then
            Assert.IsTrue(success);
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);

            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            _testFileReader.Verify(x => x.Read(It.IsAny<TextReader>()), Times.Once);
        }

        [Test]
        public void GetRawFile_should_return_correct_xml()
        {
            // given + when
            var xml = _testRepository.GetRawFile("filePath.xml");

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("<xml></xml>", xml);
        }

        [Test]
        public void DeleteFile_should_return_true_if_file_deleted()
        {
            // given + when
            _fileHandler.Setup(x => x.DeleteFile(It.IsAny<string>())).Returns(true);
            var deleteFile = _testRepository.DeleteFile(It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(deleteFile);
        }

        [Test]
        public void DeleteFile_should_return_false_if_file_did_not_deleted()
        {
            // given + when
            _fileHandler.Setup(x => x.DeleteFile(It.IsAny<string>())).Returns(false);
            var deleteFile = _testRepository.DeleteFile(It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            Assert.IsFalse(deleteFile);
        }
    }
}

using Moq;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Service.Controllers;

namespace Syringe.Tests.Unit.Service.Api
{
    [TestFixture]
    public class TestsControllerTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void CopyTest_should_save_test_file_with_additional_test(bool expectedResult)
        {
            // given
            const int expectedPosition = 4;
            const string expectedFilename = "I AM YO FILENAME BRO.FML";

            var testRepositoryMock = new Mock<ITestRepository>();

            var expectedExistingTest = new Test { Description = "Initial description" };
            testRepositoryMock
                .Setup(x => x.GetTest(expectedFilename, expectedPosition))
                .Returns(expectedExistingTest);

            testRepositoryMock
                .Setup(x => x.CreateTest(expectedFilename, expectedExistingTest))
                .Returns(expectedResult);

            // when
            var controller = new TestsController(testRepositoryMock.Object, null);
            bool result = controller.CopyTest(expectedPosition, expectedFilename);

            // then
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(expectedExistingTest.Description, Is.EqualTo("Copy of Initial description"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CopyTestFile_should_save_existing_test_file_with_new_name(bool expectedResult)
        {
            // given
            const string sourceFilename = "I AM YO FILENAME BRO.FML";
            const string targetFilename = "<insert virus script here so I can break Chris' computer...>";

            var testRepositoryMock = new Mock<ITestRepository>();

            var expectedExistingTestFile = new TestFile { Filename = sourceFilename };
            testRepositoryMock
                .Setup(x => x.GetTestFile(sourceFilename))
                .Returns(expectedExistingTestFile);

            testRepositoryMock
                .Setup(x => x.CreateTestFile(expectedExistingTestFile))
                .Returns(expectedResult);

            // when
            var controller = new TestsController(testRepositoryMock.Object, null);
            bool result = controller.CopyTestFile(sourceFilename, targetFilename);

            // then
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(expectedExistingTestFile.Filename, Is.EqualTo(targetFilename));
        }

        [Test]
        public void TestsController_should_be_decorated_with_UnitOfWorkAttribute()
        {
            // given + when + then
            Assert.IsTrue(typeof(TestsController).IsDefined(typeof(UnitOfWorkAttribute), false));
        }
    }
}
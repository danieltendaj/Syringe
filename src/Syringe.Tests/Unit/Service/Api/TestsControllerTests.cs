using Moq;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Service.Api;

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
                .Setup(x => x.CreateTest(expectedExistingTest))
                .Returns(expectedResult);

            // when
            var controller = new TestsController(testRepositoryMock.Object, null);
            bool result = controller.CopyTest(expectedPosition, expectedFilename);

            // then
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(expectedExistingTest.Description, Is.EqualTo("Copy of Initial description"));
        }
    }
}
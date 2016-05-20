using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Environment;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Web.Controllers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class TestFileControllerTests
    {
        private TestFileController _testFileController;
        private Mock<ITestService> _testServiceMock;
        private Mock<IEnvironmentsService> _environmentsService;

        [SetUp]
        public void Setup()
        {
            _testServiceMock = new Mock<ITestService>();
            _environmentsService = new Mock<IEnvironmentsService>();

            _testServiceMock.Setup(x => x.GetTestFile(It.IsAny<string>())).Returns(new TestFile());
            _testServiceMock.Setup(x => x.UpdateTestVariables(It.IsAny<TestFile>())).Returns(true);
            _testServiceMock.Setup(x => x.CreateTestFile(It.IsAny<TestFile>())).Returns(true);
            _testServiceMock.Setup(x => x.DeleteFile(It.IsAny<string>())).Returns(true);
            _testFileController = new TestFileController(_testServiceMock.Object, _environmentsService.Object);
        }

        [Test]
        public void Add_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testFileController.Add() as ViewResult;

            // then
            Assert.AreEqual("Add", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }
        [Test]
        public void Add_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            _testFileController.ModelState.Clear();

            // when
            var redirectToRouteResult = _testFileController.Add(new TestFileViewModel()) as RedirectToRouteResult;

            // then
            _testServiceMock.Verify(x => x.CreateTestFile(It.IsAny<TestFile>()), Times.Once);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual("Home", redirectToRouteResult.RouteValues["controller"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testFileController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testFileController.Add(new TestFileViewModel()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.CreateTestFile(It.IsAny<TestFile>()), Times.Never);
            Assert.AreEqual("Add", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_update_failed()
        {
            // given
            _testServiceMock.Setup(x => x.CreateTestFile(It.IsAny<TestFile>())).Returns(false);

            // when
            var viewResult = _testFileController.Add(new TestFileViewModel()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.CreateTestFile(It.IsAny<TestFile>()), Times.Once);
            Assert.AreEqual("Add", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }


        [Test]
        public void Update_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testFileController.Update(It.IsAny<string>()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.GetTestFile(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Update", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Update_should_redirect_to_view_when_validation_succeeded()
        {
            // given + when
            var redirectToRouteResult = _testFileController.Update(new TestFileViewModel()) as RedirectToRouteResult;

            // then
            _testServiceMock.Verify(x => x.UpdateTestVariables(It.IsAny<TestFile>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual("Test", redirectToRouteResult.RouteValues["controller"]);
        }

        [Test]
        public void Update_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testFileController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testFileController.Update(new TestFileViewModel()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.UpdateTestVariables(It.IsAny<TestFile>()), Times.Never);
            Assert.AreEqual("Update", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Update_should_return_correct_view_and_model_when_update_failed()
        {
            // given
            _testServiceMock.Setup(x => x.UpdateTestVariables(It.IsAny<TestFile>())).Returns(false);

            // when
            var viewResult = _testFileController.Update(new TestFileViewModel()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.UpdateTestVariables(It.IsAny<TestFile>()), Times.Once);
            Assert.AreEqual("Update", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void AddHeaderItem_should_return_correct_view_and_model()
        {
            // given 
            var environments = new[]
            {
                new Environment { Name = "Env2", Order = 2},
                new Environment { Name = "Env3", Order = 1},
            };

            _environmentsService
                .Setup(x => x.List())
                .Returns(environments);

            // when
            var viewResult = _testFileController.AddVariableItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/VariableViewModel", viewResult.ViewName);
            Assert.IsInstanceOf<VariableViewModel>(viewResult.Model);

            var variableViewModel = viewResult.Model as VariableViewModel;
            Assert.That(variableViewModel.AvailableEnvironments.Length, Is.EqualTo(2));

            Assert.That(variableViewModel.AvailableEnvironments[0].Text, Is.EqualTo("Env3"));
            Assert.That(variableViewModel.AvailableEnvironments[0].Value, Is.EqualTo("Env3"));
            Assert.That(variableViewModel.AvailableEnvironments[0].Disabled, Is.False);

            Assert.That(variableViewModel.AvailableEnvironments[1].Text, Is.EqualTo("Env2"));
            Assert.That(variableViewModel.AvailableEnvironments[1].Value, Is.EqualTo("Env2"));
            Assert.That(variableViewModel.AvailableEnvironments[1].Disabled, Is.False);
        }

        [Test]
        public void Delete_should_redirect_to_view_when_file_deleted()
        {
            // given + when
            var redirectToRouteResult = _testFileController.Delete(It.IsAny<string>()) as RedirectToRouteResult;

            // then
            _testServiceMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual("Home", redirectToRouteResult.RouteValues["controller"]);
        }
        
        [Test]
        public void Copy_should_return_correct_redirection_to_view()
        {
            // given
            const string sourceFileName = "doobeedoo.dont.touch.me.there";
            const string targetFileName = "yum yum in my tum";

            // when
            var redirectToRouteResult = _testFileController.Copy(sourceFileName, targetFileName) as RedirectToRouteResult;

            // then
            _testServiceMock.Verify(x => x.CopyTestFile(sourceFileName, targetFileName), Times.Once);

            Assert.That(redirectToRouteResult, Is.Not.Null);
            Assert.That(redirectToRouteResult.RouteValues["action"], Is.EqualTo("Index"));
            Assert.That(redirectToRouteResult.RouteValues["controller"], Is.EqualTo("Home"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Web.Controllers;
using Syringe.Web.Mappers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class TestControllerTests
    {
        private TestController _testController;
        private Mock<ITestService> _testServiceMock;
        private Mock<IUserContext> _userContextMock;
        private Mock<ITestFileMapper> _testFileMapperMock;
        private Mock<IEnvironmentsService> _environmentService;

        [SetUp]
        public void Setup()
        {
            _testServiceMock = new Mock<ITestService>();
            _userContextMock = new Mock<IUserContext>();
            _testFileMapperMock = new Mock<ITestFileMapper>();
            _environmentService = new Mock<IEnvironmentsService>();

            _testFileMapperMock.Setup(x => x.BuildTests(It.IsAny<IEnumerable<Test>>()));
            _testFileMapperMock.Setup(x => x.BuildViewModel(It.IsAny<Test>())).Returns(new TestViewModel());
            _testFileMapperMock.Setup(x => x.BuildVariableViewModel(It.IsAny<TestFile>())).Returns(new List<VariableViewModel>());
            _testServiceMock.Setup(x => x.GetTestFile(It.IsAny<string>())).Returns(new TestFile());
            _testServiceMock.Setup(x => x.GetTest(It.IsAny<string>(), It.IsAny<int>()));
            _testServiceMock.Setup(x => x.DeleteTest(It.IsAny<int>(), It.IsAny<string>()));
            _testServiceMock.Setup(x => x.EditTest(It.IsAny<Test>()));
            _testServiceMock.Setup(x => x.CreateTest(It.IsAny<Test>()));

            _testController = new TestController(_testServiceMock.Object, _userContextMock.Object, _testFileMapperMock.Object, _environmentService.Object);
        }

        [Test]
        public void View_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.View(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.GetTestFile(It.IsAny<string>()), Times.Once);
            _testFileMapperMock.Verify(x => x.BuildTests(It.IsAny<IEnumerable<Test>>()), Times.Once);
            Assert.AreEqual("View", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.Edit(It.IsAny<string>(), It.IsAny<int>()) as ViewResult;

            // then
            _testServiceMock.Verify(x => x.GetTest(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _testFileMapperMock.Verify(x => x.BuildViewModel(It.IsAny<Test>()), Times.Once);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            _testController.ModelState.Clear();
            
            // when
            var redirectToRouteResult = _testController.Edit(new TestViewModel()) as RedirectToRouteResult;

            // then
            _testFileMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Once);
            _testServiceMock.Verify(x => x.EditTest(It.IsAny<Test>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testController.Edit(new TestViewModel()) as ViewResult;

            // then
            _testFileMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Never);
            _testServiceMock.Verify(x => x.EditTest(It.IsAny<Test>()), Times.Never);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void ViewXml_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.ViewXml(It.IsAny<string>()) as ViewResult;

            // then 
            _testServiceMock.Verify(x => x.GetXml(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("ViewXml", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Delete_should_return_correct_redirection_to_view()
        {
            // given + when
            var redirectToRouteResult = _testController.Delete(It.IsAny<int>(), It.IsAny<string>()) as RedirectToRouteResult;

            // then
            _testServiceMock.Verify(x => x.DeleteTest(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.Add(It.IsAny<string>()) as ViewResult;

            // then
            Assert.AreEqual("Edit", viewResult.ViewName);
            _testFileMapperMock.Verify(x => x.BuildVariableViewModel(It.IsAny<TestFile>()), Times.Once);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void Add_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            _testController.ModelState.Clear();

            // when
            var redirectToRouteResult = _testController.Add(new TestViewModel()) as RedirectToRouteResult;

            // then
            _testFileMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Once);
            _testServiceMock.Verify(x => x.CreateTest(It.IsAny<Test>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testController.Add(new TestViewModel()) as ViewResult;

            // then
            _testFileMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Never);
            _testServiceMock.Verify(x => x.CreateTest(It.IsAny<Test>()), Times.Never);
            Assert.AreEqual("Edit", viewResult.ViewName);

        }

        [Test]
        public void AddVerification_should_return_correct_view()
        {
            // given + when
            var viewResult = _testController.AddAssertion() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/AssertionViewModel", viewResult.ViewName);
            Assert.IsInstanceOf<AssertionViewModel>(viewResult.Model);
        }

        [Test]
        public void AddParseResponseItem_should_return_correct_view()
        {
            // given + when
            var viewResult = _testController.AddCapturedVariableItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/CapturedVariableItem", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.CapturedVariableItem>(viewResult.Model);
        }

        [Test]
        public void AddHeaderItem_should_return_correct_view()
        {
            // given + when
            var viewResult = _testController.AddHeaderItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/HeaderItem", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.HeaderItem>(viewResult.Model);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Helpers;
using Syringe.Core.Services;
using Syringe.Core.Tests.Results;
using Syringe.Web.Controllers;

namespace Syringe.Tests.Unit.Web.Controllers
{
    [TestFixture]
    public class ResultsControllerTests
    {
        private Mock<IUrlHelper> _urlHelperMock;
        private Mock<ITestService> _testsClient;
        private Mock<IEnvironmentsService> _environmentServiceClient;

        private ResultsController _resultsController;

        [SetUp]
        public void Setup()
        {
            _urlHelperMock = new Mock<IUrlHelper>();
            _environmentServiceClient = new Mock<IEnvironmentsService>();

            _testsClient = new Mock<ITestService>();
            _testsClient.Setup(x => x.GetResultById(It.IsAny<Guid>())).Returns(new TestFileResult());
            _testsClient.Setup(x => x.GetSummaries(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(new TestFileResultSummaryCollection()));
            
            _resultsController = new ResultsController(_urlHelperMock.Object, _testsClient.Object, _environmentServiceClient.Object);
        }

        [Test]
        public void Index_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _resultsController.Index().Result as ViewResult;

            // then
            _testsClient.Verify(x => x.GetSummaries(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.AreEqual(viewResult.ViewBag.Title, "All");
            Assert.IsInstanceOf<TestFileResultSummaryCollection>(viewResult.Model);
        }

        [Test]
        public void Today_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _resultsController.Today().Result as ViewResult;

            // then
            _testsClient.Verify(x => x.GetSummaries(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.AreEqual(viewResult.ViewBag.Title, "All");
            Assert.IsInstanceOf<TestFileResultSummaryCollection>(viewResult.Model);
        }

        [Test]
        [TestCase("Chris Loves Simon")]
        [TestCase("Hemang is Superman")]
        public void Index_should_return_correct_viewbag_title_based_on_environment(string environment)
        {
            // given + when
            var viewResult = _resultsController.Index(1, 20, environment).Result as ViewResult;

            // then
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [Test]
        [TestCase("Chris Loves Simon")]
        [TestCase("Hemang is Superman")]
        public void Today_should_return_correct_viewbag_title_based_on_environment(string environment)
        {
            // given + when
            var viewResult = _resultsController.Today(1, 20, environment).Result as ViewResult;

            // then
            Assert.AreEqual("Index", viewResult.ViewName);
        }


        [Test]
        public void ViewResult_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _resultsController.ViewResult(It.IsAny<Guid>()) as ViewResult;

            // then
            _testsClient.Verify(x => x.GetResultById(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual("ViewResult", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileResult>(viewResult.Model);
        }

        [Test]
        public void DeleteResult_should_call_delete_methods_and_redirect_to_correct_action()
        {
            // given + when
            var redirectToRouteResult = _resultsController.Delete(It.IsAny<Guid>()) as RedirectToRouteResult;

            // then
            _testsClient.Verify(x => x.GetResultById(It.IsAny<Guid>()), Times.Once);
            _testsClient.Verify(x => x.DeleteResult(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
using Syringe.Core.Helpers;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests.Results;
using Syringe.Tests.StubsMocks;
using Syringe.Web.Controllers;
using Syringe.Web.Models;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ITestService> _testsClient;
        private Mock<Func<IRunViewModel>> _runViewModelFactory;
        private Mock<IEnvironmentsService> _environmentService;
        private HomeController _homeController;
        private HealthCheckMock _mockHealthCheck;

        [SetUp]
        public void Setup()
        {
            var userContext = new Mock<IUserContext>();
            _mockHealthCheck = new HealthCheckMock();
            _environmentService = new Mock<IEnvironmentsService>();

            var urlHelper = new Mock<IUrlHelper>();

            var runViewModelMockService = new Mock<IRunViewModel>();
            runViewModelMockService.Setup(x => x.Run(It.IsAny<UserContext>(), It.IsAny<string>(), It.IsAny<string>()));
            _runViewModelFactory = new Mock<Func<IRunViewModel>>();
            _runViewModelFactory.Setup(x => x()).Returns(runViewModelMockService.Object);

            _testsClient = new Mock<ITestService>();
            _testsClient.Setup(x => x.GetResultById(It.IsAny<Guid>())).Returns(new TestFileResult());
            _testsClient.Setup(x => x.GetSummaries()).Returns(new List<TestFileResultSummary>());
            _testsClient.Setup(x => x.GetSummariesForToday()).Returns(new List<TestFileResultSummary>());

            _homeController = new HomeController(_testsClient.Object, userContext.Object, _runViewModelFactory.Object, _mockHealthCheck, urlHelper.Object, _environmentService.Object);
        }

        [Test]
        public void AllResults_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _homeController.AllResults() as ViewResult;

            // then
            _testsClient.Verify(x => x.GetSummaries(), Times.Once);
            Assert.AreEqual("AllResults", viewResult.ViewName);
            Assert.IsInstanceOf<IEnumerable<TestFileResultSummary>>(viewResult.Model);
        }

        [Test]
        public void TodaysResults_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _homeController.TodaysResults() as ViewResult;

            // then
            _testsClient.Verify(x => x.GetSummariesForToday(), Times.Once);
            Assert.AreEqual("AllResults", viewResult.ViewName);
            Assert.IsInstanceOf<IEnumerable<TestFileResultSummary>>(viewResult.Model);
        }


        [Test]
        public void ViewResult_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _homeController.ViewResult(It.IsAny<Guid>()) as ViewResult;

            // then
            _testsClient.Verify(x => x.GetResultById(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual("ViewResult", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileResult>(viewResult.Model);
        }

        [Test]
        public async void DeleteResult_should_call_delete_methods_and_redirect_to_correct_action()
        {
            // given + when
            var redirectToRouteResult = await _homeController.DeleteResult(It.IsAny<Guid>()) as RedirectToRouteResult;

            // then
            _testsClient.Verify(x => x.GetResultById(It.IsAny<Guid>()), Times.Once);
            _testsClient.Verify(x => x.DeleteResultAsync(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual("AllResults", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Run_should_call_run_method_and_return_correct_model()
        {
            // given + when
            var viewResult = _homeController.Run(It.IsAny<string>(), It.IsAny<string>()) as ViewResult;

            // then
            _runViewModelFactory.Verify(x => x(), Times.Once);
            Assert.AreEqual("Run", viewResult.ViewName);
            Assert.IsInstanceOf<IRunViewModel>(viewResult.Model);
        }

        [Test]
        public void Index_should_throw_HealthCheckException_if_healthcheck_fails()
        {
            // given
            _mockHealthCheck.ThrowsException = true;

            // when + then
            Assert.Throws<HealthCheckException>(() => _homeController.Index());
        }

        [Test]
        public void Index_should_call_run_method_and_return_correct_model()
        {
            // given
            var environments = new List<Environment>
            {
                new Environment { Name = "Middle", Order = 1 },
                new Environment { Name = "End", Order = 2 },
                new Environment { Name = "First", Order = 0 },
            };

            _environmentService
                .Setup(x => x.List())
                .Returns(environments);

            // when
            var viewResult = _homeController.Index(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            _testsClient.Verify(x => x.ListFiles(), Times.Once);
            Assert.AreEqual("Index", viewResult.ViewName);

            var model = viewResult.Model as IndexViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Environments, Is.EqualTo(new[] { "First", "Middle", "End" }));
        }
    }
}

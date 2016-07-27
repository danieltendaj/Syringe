using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;
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
	    private JsonConfiguration _configuration;

	    [SetUp]
        public void Setup()
        {
            _mockHealthCheck = new HealthCheckMock();
            _environmentService = new Mock<IEnvironmentsService>();
			_configuration = new JsonConfiguration();

			var runViewModelMockService = new Mock<IRunViewModel>();
            runViewModelMockService.Setup(x => x.Run(It.IsAny<UserContext>(), It.IsAny<string>(), It.IsAny<string>()));
            _runViewModelFactory = new Mock<Func<IRunViewModel>>();
            _runViewModelFactory.Setup(x => x()).Returns(runViewModelMockService.Object);

            _testsClient = new Mock<ITestService>();
            _testsClient.Setup(x => x.GetResultById(It.IsAny<Guid>())).Returns(new TestFileResult());
            _homeController = new HomeController(_testsClient.Object, _runViewModelFactory.Object, _mockHealthCheck, _environmentService.Object, _configuration);
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
		public void Index_should_return_view_name()
		{
			// given + when
			var viewResult = _homeController.Index() as ViewResult;

			// then
			Assert.AreEqual("Index", viewResult.ViewName);
		}

		[Test]
		public void Index_should_return_readonly_view_name_when_configuration_is_readonly()
		{
			// given + when
			_configuration.ReadonlyMode = true;
			var viewResult = _homeController.Index() as ViewResult;

			// then
			Assert.AreEqual("Index-ReadonlyMode", viewResult.ViewName);
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
            var view = _homeController.Index(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            _testsClient.Verify(x => x.ListFiles(), Times.Once);
            Assert.AreEqual("Index", view.ViewName);

            var model = view.Model as IndexViewModel;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Environments, Is.EqualTo(new[] { "First", "Middle", "End" }));
        }
    }
}

using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Moq;
using NUnit.Framework;
using Syringe.Service.Controllers;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Service.Controllers
{
    public class UnitOfWorkAttributeTests
    {
        [Test]
        public void should_ignore_when_controller_is_not_testscontroller()
        {
            // given
            var attribute = new UnitOfWorkAttribute();
            var httpActionExecutedContext = new HttpActionExecutedContext();
            var repository = new TestFileResultRepositoryMock();
            var configurationController = new ConfigurationController(null, null, null);

            var actionContext = new Mock<HttpActionContext>();
            actionContext.Object.ControllerContext = new HttpControllerContext();
            actionContext.Object.ControllerContext.Controller = configurationController;
            httpActionExecutedContext.ActionContext = actionContext.Object;

            // when
            attribute.OnActionExecuted(httpActionExecutedContext);

            // then
            Assert.That(repository.Disposed, Is.False);
        }

        [Test]
        public void should_dispose_repository()
        {
            // given
            var attribute = new UnitOfWorkAttribute();
            var httpActionExecutedContext = new HttpActionExecutedContext();
            var repository = new TestFileResultRepositoryMock();
            var testsController = new TestsController(null, repository);

            var actionContext = new Mock<HttpActionContext>();
            actionContext.Object.ControllerContext = new HttpControllerContext();
            actionContext.Object.ControllerContext.Controller = testsController;
            httpActionExecutedContext.ActionContext = actionContext.Object;

            // when
            attribute.OnActionExecuted(httpActionExecutedContext);

            // then
            Assert.That(repository.Disposed, Is.True);
        }
    }
}
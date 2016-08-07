using System.Web.Http.Filters;

namespace Syringe.Service.Controllers
{
    public class UnitOfWorkAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = ((TestsController)actionExecutedContext.ActionContext.ControllerContext.Controller);
            controller.TestFileResultRepository.Dispose();
        }
    }
}
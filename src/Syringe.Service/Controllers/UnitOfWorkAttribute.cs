using System.Web.Http.Filters;

namespace Syringe.Service.Controllers
{
    /// <summary>
    /// Ensures that the controller's repository is disposed when the request ends,
    /// so all changes are flushed to disk.
    /// </summary>
    public class UnitOfWorkAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as TestsController;

            if (controller != null)
                controller.TestFileResultRepository.Dispose();
        }
    }
}
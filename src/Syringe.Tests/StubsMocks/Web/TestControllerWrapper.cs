using System.Reflection;
using System.Web.Mvc;
using Syringe.Web.Controllers;

namespace Syringe.Tests.StubsMocks.Web
{
    /// <summary>
    /// Exposes the OnActionExecuting method for testing
    /// </summary>
    public class TestControllerWrapper
    {
        private readonly TestController _testController;

        public TestControllerWrapper(TestController testController)
        {
            _testController = testController;
        }

        public void ExecuteOnActionExecuting(ActionExecutingContext filterContext)
        {
            MethodInfo info = _testController.GetType()
                                          .GetMethod("OnActionExecuting",
                                                      BindingFlags.NonPublic
                                                      | BindingFlags.Instance);

            info.Invoke(_testController, new object[] { filterContext });
        }
    }
}
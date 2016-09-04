using System.Web.Mvc;
using System.Web.Mvc.Async;
using StructureMap;

namespace Syringe.Web.DependencyResolution
{
    public class InjectingActionInvoker : AsyncControllerActionInvoker
    {
        private readonly IContainer _container;

        public InjectingActionInvoker(IContainer container)
        {
            _container = container;
        }

        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var info = base.GetFilters(controllerContext, actionDescriptor);

            foreach (var filter in info.AuthorizationFilters)
            {
                _container.BuildUp(filter);
            }

            foreach (var filter in info.ActionFilters)
            {
                _container.BuildUp(filter);
            }

            foreach (var filter in info.ResultFilters)
            {
                _container.BuildUp(filter);
            }

            foreach (var filter in info.ExceptionFilters)
            {
                _container.BuildUp(filter);
            }

            return info;
        }
    }
}
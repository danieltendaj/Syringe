using System.Collections.Generic;
using System.Web.Mvc;
using StructureMap;

namespace Syringe.Web.DependencyResolution
{
    /// <summary>
    /// https://github.com/roadkillwiki/roadkill/blob/master/src/Roadkill.Core/DependencyResolution/MVC/MvcAttributeProvider.cs
    /// </summary>
    internal class MvcAttributeProvider : FilterAttributeFilterProvider, IFilterProvider
    {
        private readonly IContainer _container;

        public MvcAttributeProvider(IContainer container)
        {
            _container = container;
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            IEnumerable<FilterAttribute> filters = base.GetControllerAttributes(controllerContext, actionDescriptor);

            foreach (FilterAttribute filter in filters)
            {
                _container.BuildUp(filter);
            }

            return filters;
        }

        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            IEnumerable<FilterAttribute> filters = base.GetActionAttributes(controllerContext, actionDescriptor);

            foreach (FilterAttribute filter in filters)
            {
                _container.BuildUp(filter);
            }

            return filters;
        }

        public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            IEnumerable<Filter> filters = base.GetFilters(controllerContext, actionDescriptor);

            foreach (Filter filter in filters)
            {
                // Injects the instance with Structuremap's dependencies
                //Log.Information(filter.Instance.GetType().Name);
                _container.BuildUp(filter.Instance);
            }

            return filters;
        }
    }
}
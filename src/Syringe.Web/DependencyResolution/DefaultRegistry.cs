// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using StructureMap;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Helpers;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Mappers;
using Syringe.Web.Models;

namespace Syringe.Web.DependencyResolution
{
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;

    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

			MvcConfiguration config = MvcConfiguration.Load();
			string serviceUrl = config.ServiceUrl;

			For<IRunViewModel>().Use<RunViewModel>();
            For<ITestFileMapper>().Use<TestFileMapper>();
            For<IUserContext>().Use<UserContext>();
            For<IUrlHelper>().Use<UrlHelper>();
            For<ITestService>().Use(() => new TestsClient(serviceUrl));
            For<ITasksService>().Use(() => new TasksClient(serviceUrl));
	        For<IHealthCheck>().Use(() => new HealthCheck(serviceUrl));
            For<IEnvironmentsService>().Use(() => new EnvironmentsClient(serviceUrl));
        }
    }
}
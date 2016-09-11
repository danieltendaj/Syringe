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

using System.Web.Mvc;
using StructureMap;
using StructureMap.Graph;
using Syringe.Client;
using Syringe.Client.RestSharpHelpers;
using Syringe.Core.Configuration;
using Syringe.Core.Helpers;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Web.Configuration;
using Syringe.Web.Mappers;
using Syringe.Web.Models;
using UrlHelper = Syringe.Core.Helpers.UrlHelper;

namespace Syringe.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry() : this(null)
        { }

        internal DefaultRegistry(IConfiguration configuration)
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.Assembly("Syringe.Core");
                scan.WithDefaultConventions();
                scan.With(new ControllerConvention());
            });

            For<IActionInvoker>().Use<InjectingActionInvoker>();
            Policies.SetAllProperties(c =>
            {
                c.OfType<IActionInvoker>();
                c.WithAnyTypeFromNamespaceContainingType<IConfiguration>();
            });

            SetupConfiguration(configuration);
            SetupModelHelpers();
            SetupRestClients();

            For<IEncryption>()
                .Use(x => new AesEncryption(x.GetInstance<IConfiguration>().EncryptionKey));
        }

        private void SetupConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                For<IConfiguration>()
                    .Use(x => x.GetInstance<IConfigurationService>().GetConfiguration())
                    .Singleton();
            }
            else
            {
                For<IConfiguration>()
                    .Use(configuration);
            }

            For<IMvcConfigurationProvider>()
                .Use<MvcConfigurationProvider>()
                .Singleton();
            For<MvcConfiguration>()
                .Use(x => x.GetInstance<IMvcConfigurationProvider>().Load());
            For<IConfigurationService>()
                .Use(x => new ConfigurationClient(x.GetInstance<MvcConfiguration>().ServiceUrl));
        }

        private void SetupModelHelpers()
        {
            For<IRunViewModel>().Use<RunViewModel>();
            For<ITestFileMapper>().Use<TestFileMapper>();
            For<IUserContext>().Use<UserContext>();
            For<IUrlHelper>().Use<UrlHelper>();
        }

        private void SetupRestClients()
        {
            For<IRestSharpClientFactory>().Use<RestSharpClientFactory>();
            For<ITestService>().Use(x => new TestsClient(x.GetInstance<MvcConfiguration>().ServiceUrl, x.GetInstance<IRestSharpClientFactory>()));
            For<ITasksService>().Use(x => new TasksClient(x.GetInstance<MvcConfiguration>().ServiceUrl));
            For<IHealthCheck>().Use(x => new HealthCheck(x.GetInstance<MvcConfiguration>().ServiceUrl));
            For<IEnvironmentsService>().Use(x => new EnvironmentsClient(x.GetInstance<MvcConfiguration>().ServiceUrl));
        }
    }
}
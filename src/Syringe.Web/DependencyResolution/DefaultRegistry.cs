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

namespace Syringe.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry() : this(null, null)
        {
        }

        internal DefaultRegistry(MvcConfiguration mvcConfig, IConfigurationService configurationService)
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.Assembly("Syringe.Core");
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

            //
            // Configration - load from the service at startup, cache it.
            //
            //if (mvcConfig == null)
            //	mvcConfig = MvcConfiguration.Load();

            //         string serviceUrl = mvcConfig.ServiceUrl;
            //         For<MvcConfiguration>().Use(mvcConfig);

            //if (configurationService == null)
            //	configurationService = new ConfigurationClient(serviceUrl);

            For<IMvcConfigurationProvider>()
                .Singleton();
            For<MvcConfiguration>()
                .Use(x => x.GetInstance<IMvcConfigurationProvider>().Load());
            For<IConfigurationService>()
                .Use(x => new ConfigurationClient(x.GetInstance<MvcConfiguration>().ServiceUrl))
                .Singleton();
            For<IConfiguration>()
                .Use(x => x.GetInstance<IConfigurationService>().GetConfiguration())
                .Singleton();
            For<IEncryption>()
                .Use(x => new RijndaelEncryption(x.GetInstance<IConfiguration>().EncryptionKey));

            //
            // Model helpers
            //
            For<IRunViewModel>().Use<RunViewModel>();
            For<ITestFileMapper>().Use<TestFileMapper>();
            For<IUserContext>().Use<UserContext>();
            For<IUrlHelper>().Use<UrlHelper>();

            //
            // REST API clients
            //
            For<IRestSharpClientFactory>().Use<RestSharpClientFactory>();
            For<ITestService>().Use(x => new TestsClient(x.GetInstance<MvcConfiguration>().ServiceUrl, x.GetInstance<IRestSharpClientFactory>()));
            For<ITasksService>().Use(x => new TasksClient(x.GetInstance<MvcConfiguration>().ServiceUrl));
            For<IHealthCheck>().Use(x => new HealthCheck(x.GetInstance<MvcConfiguration>().ServiceUrl));
            For<IEnvironmentsService>().Use(x => new EnvironmentsClient(x.GetInstance<MvcConfiguration>().ServiceUrl));
        }
    }
}
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

using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Octopus.Client;
using StructureMap;
using StructureMap.Graph;
using Syringe.Core.Configuration;
using Syringe.Core.Environment;
using Syringe.Core.IO;
using Syringe.Core.Tests.Repositories;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Service.Api.Hubs;
using Syringe.Service.Parallel;
using WebApiContrib.IoC.StructureMap;

namespace Syringe.Service.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.Assembly("Syringe.Core");
                    scan.WithDefaultConventions();
                });

            For<IDependencyResolver>().Use<StructureMapSignalRDependencyResolver>().Singleton();
            For<System.Web.Http.Dependencies.IDependencyResolver>().Use<StructureMapResolver>();

            For<Startup>().Use<Startup>().Singleton();
            For<TaskMonitorHub>().Use<TaskMonitorHub>();

            // Configuration: load the configuration from the store
            var configStore = new JsonConfigurationStore();
            IConfiguration configuration = configStore.Load();
            For<IConfigurationStore>().Use(configStore).Singleton();
            For<IConfiguration>().Use(configuration);

            For<ITestFileResultRepository>().Use<LiteDbTestFileRepository>().Singleton();
            For<ITestFileQueue>().Use<ParallelTestFileQueue>().Singleton();
            Forward<ITestFileQueue, ITaskObserver>();

            For<ITaskPublisher>().Use<TaskPublisher>().Singleton();
            For<ITaskGroupProvider>().Use<TaskGroupProvider>().Singleton();

            SetupTestFileFormat(configuration);
            SetupEnvironmentSource(configuration);

            For<IHubConnectionContext<ITaskMonitorHubClient>>()
                .Use(context => context.GetInstance<IDependencyResolver>().Resolve<IConnectionManager>().GetHubContext<TaskMonitorHub, ITaskMonitorHubClient>().Clients);
        }

        private void SetupEnvironmentSource(IConfiguration configuration)
        {
            // Environments, use Octopus if keys exist
            bool containsOctopusApiKey = !string.IsNullOrEmpty(configuration.OctopusConfiguration?.OctopusApiKey);
            bool containsOctopusUrl = !string.IsNullOrEmpty(configuration.OctopusConfiguration?.OctopusUrl);

            if (containsOctopusApiKey && containsOctopusUrl)
            {
                For<IOctopusRepositoryFactory>().Use<OctopusRepositoryFactory>();
                For<IOctopusRepository>().Use(x => x.GetInstance<IOctopusRepositoryFactory>().Create());
                For<IEnvironmentProvider>().Use<OctopusEnvironmentProvider>();
            }
            else
            {
                For<IEnvironmentProvider>().Use<JsonEnvironmentProvider>();
            }
        }

        private void SetupTestFileFormat(IConfiguration configuration)
        {
            For<IFileHandler>().Use<FileHandler>();
            For<ITestRepository>().Use<TestRepository>();

            // Test file readers and writers - set to json by default as there is no alternative right now.
            switch (configuration.TestFileFormat)
            {
                default:
                    For<ITestFileReader>().Use<Core.Tests.Repositories.Json.Reader.TestFileReader>();
                    For<ITestFileWriter>().Use<Core.Tests.Repositories.Json.Writer.TestFileWriter>();
                    break;
            }
        }
    }
}
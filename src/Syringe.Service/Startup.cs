using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Core.Environment.Json;
using Syringe.Core.Tests.Variables.SharedVariables;
using Syringe.Service.DependencyResolution;

namespace Syringe.Service
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                ;//.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            ;//.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();
            services.AddMvc();
            services.AddSwaggerGen();
            services.AddMemoryCache();
            services.Configure<JsonConfiguration>(Configuration.GetSection("settings"));
            services.Configure<SharedVariables>(Configuration.GetSection("sharedVariables"));

            var container = new Container();
            container.Configure(config =>
            {
                config.AddRegistry<DefaultRegistry>();
                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseMvc();
        }
    }
}

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;
using Serilog;
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
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			// Setup Sirilog
			if (env.IsDevelopment())
			{
				Log.Logger = new LoggerConfiguration()
					.Enrich.FromLogContext()
					.WriteTo.LiterateConsole(Serilog.Events.LogEventLevel.Information,
						"[{Timestamp}] [Website] {Message}{NewLine}{Exception}")
					.CreateLogger();
			}
			else
			{
				// Remove the colour logging for production, it fills the logs with "\[n" etc.
				Log.Logger = new LoggerConfiguration()
					.Enrich.FromLogContext()
					.WriteTo.Console(Serilog.Events.LogEventLevel.Information,
						"[{Timestamp}] [Website] {Message}{NewLine}{Exception}")
					.CreateLogger();
			}
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices2(IServiceCollection services)
		{
			// Add framework services.
			services.AddOptions();
			services.AddMvc();
			services.AddMemoryCache();
			services.Configure<JsonConfiguration>(Configuration.GetSection("settings"));
			services.Configure<SharedVariables>(Configuration.GetSection("sharedVariables"));
			services.Configure<Environments>(Configuration.GetSection("environments"));

			var container = new Container();
			container.Configure(config =>
			{
				config.AddRegistry<ServiceRegistry>();
				config.Populate(services);
			});
			Log.Information(container.WhatDoIHave());

			return container.GetInstance<IServiceProvider>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			//loggerFactory.AddSerilog();
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseStaticFiles();
			app.UseSwagger(Assembly.GetEntryAssembly(), new SwaggerSettings());
			app.UseSwaggerUi(Assembly.GetEntryAssembly(), new SwaggerUiSettings());
			app.UseMvc();
		}
	}
}
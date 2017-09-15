using System;
using System.IO;
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
		public IConfigurationRoot Configuration { get; set; }

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			// Setup Sirilog
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Console(Serilog.Events.LogEventLevel.Information,
					"[{Timestamp}] [Website] {Message}{NewLine}{Exception}")
				.CreateLogger();
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
			services.AddOptions();

			services.Configure<Settings>(Configuration.GetSection("Settings"));
			services.Configure<SharedVariables>(Configuration.GetSection("SharedVariables"));
			services.Configure<Environments>(Configuration.GetSection("Environments"));

			var container = new Container();
			container.Configure(config =>
			{
				var settings = new Settings();
				Configuration.GetSection("Settings").Bind(settings);

				config.AddRegistry(new ServiceRegistry(settings));
				config.Populate(services);
			});

			container.AssertConfigurationIsValid();

			return container.GetInstance<IServiceProvider>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			//loggerFactory.AddSerilog();
			loggerFactory.AddConsole();
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();
			app.UseSwagger(Assembly.GetEntryAssembly(), new SwaggerSettings());
			app.UseSwaggerUi(Assembly.GetEntryAssembly(), new SwaggerUiSettings());
			app.UseMvc();
		}
	}
}
using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;
using Serilog;
using StructureMap;
using Syringe.Core.Configuration;
using Syringe.Service.DependencyResolution;

namespace Syringe.Service
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// ConfigurationBuilder is in JsonConfigurationStore.

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

			var container = new Container();
			container.Configure(config =>
			{
				var jsonConfigurationStore = new JsonConfigurationStore();
				config.AddRegistry(new ServiceRegistry(jsonConfigurationStore));
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
using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Hosting;
using Owin;
using Swashbuckle.Application;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;
using Syringe.Service.Jobs;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace Syringe.Service
{
	public class Startup
	{
		protected IDisposable WebApplication;
		private readonly IDependencyResolver _webDependencyResolver;
		private readonly IConfiguration _configuration;
	    private readonly IDbCleanupJob _cleanupJob;

	    public Startup(
			IDependencyResolver webDependencyResolver,
			IConfiguration configuration,
            IDbCleanupJob cleanupJob)
		{
			_webDependencyResolver = webDependencyResolver;
			_configuration = configuration;
	        _cleanupJob = cleanupJob;
		}

		public void Start()
		{
			try
			{
                _cleanupJob.Start();
                WebApplication = WebApp.Start(_configuration.ServiceUrl, Configuration);
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null && ex.InnerException.Message.ToLowerInvariant().Contains("access is denied"))
					throw new InvalidOperationException("Access denied - if you're running Visual Studio, restart it in adminsitrator mode. Otherwise is the service running as an administrator or privileges to listen on TCP ports?");

				throw;
			}
		}

		public void Stop()
		{
            _cleanupJob.Stop();
			WebApplication.Dispose();
		}

		public void Configuration(IAppBuilder application)
		{
			var httpConfiguration = new HttpConfiguration();
			httpConfiguration.EnableSwagger(swaggerConfig =>
			{
				swaggerConfig
					.SingleApiVersion("v1", "Syringe REST API")
					.Description("REST API for Syringe, this is used by the web UI.");

			}).EnableSwaggerUi();

			httpConfiguration.Services.Add(typeof(IExceptionLogger), new ServiceExceptionLogger());
			httpConfiguration.MapHttpAttributeRoutes();
			httpConfiguration.DependencyResolver = _webDependencyResolver;

			application.UseWebApi(httpConfiguration);
		}
	}

	public class ServiceExceptionLogger : ExceptionLogger
	{
		public override void Log(ExceptionLoggerContext context)
		{
			base.Log(context);
		}
	}
}
using System;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using Swashbuckle.Application;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;
using Syringe.Service.Parallel;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace Syringe.Service
{
	public class Startup
	{
		protected IDisposable WebApplication;
		private readonly IDependencyResolver _webDependencyResolver;
		private readonly IConfiguration _configuration;
		private readonly ITestFileQueue _testFileQueue;
		private readonly Microsoft.AspNet.SignalR.IDependencyResolver _signalRDependencyResolver;

		public Startup(
			IDependencyResolver webDependencyResolver,
			IConfiguration configuration,
			ITestFileQueue testFileQueue,
			Microsoft.AspNet.SignalR.IDependencyResolver signalRDependencyResolver)
		{
			_webDependencyResolver = webDependencyResolver;
			_configuration = configuration;
			_testFileQueue = testFileQueue;
			_signalRDependencyResolver = signalRDependencyResolver;
		}

		public void Start()
		{
			try
			{
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

			// Log to bin/errors.log
			Log.UseAllTargets();
			httpConfiguration.Services.Add(typeof(IExceptionLogger), new ServiceExceptionLogger());

			httpConfiguration.MapHttpAttributeRoutes();
			httpConfiguration.DependencyResolver = _webDependencyResolver;

			var corsOptions = new CorsOptions
			{
				PolicyProvider = new CorsPolicyProvider
				{
					PolicyResolver = context =>
					{
						var policy = new CorsPolicy();
						// Allow CORS requests from the web frontend
						policy.Origins.Add(_configuration.WebsiteUrl);
						policy.AllowAnyMethod = true;
						policy.AllowAnyHeader = true;
						policy.SupportsCredentials = true;
						return Task.FromResult(policy);
					}
				}
			};

			application.Map("/signalr", config =>
            {
                config.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    Resolver = _signalRDependencyResolver
                };
                config.RunSignalR(hubConfiguration);
            });

            application.UseCors(corsOptions);
			application.UseWebApi(httpConfiguration);
		}
	}

	public class ServiceExceptionLogger : ExceptionLogger
	{
		public override void Log(ExceptionLoggerContext context)
		{
			Syringe.Core.Logging.Log.Error(context.Exception, "Service exception");
			base.Log(context);
		}
	}
}
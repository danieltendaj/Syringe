using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using Syringe.Web.DependencyResolution;

namespace Syringe.Web
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();

			var container = new Container();
			container.Configure(config =>
			{
				config.AddRegistry<WebRegistry>();
				config.Populate(services);
			});

			return container.GetInstance<IServiceProvider>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			//ConfigureAuthentation(app);

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		private void ConfigureAuthentation(IApplicationBuilder app)
		{
			app.UseGoogleAuthentication(new GoogleOptions()
			{
				ClientId = Configuration["Authentication:Google:ClientId"],
				ClientSecret = Configuration["Authentication:Google:ClientSecret"]
			});

			app.UseOAuthAuthentication(new OAuthOptions()
			{
			});
		}

		//private void ConfigureOAuth(IAppBuilder app)
		//{
		//	// Call the service to get its configuration back
		//	var provider = new MvcConfigurationProvider(new ConfigLocator());
		//	MvcConfiguration mvcConfiguration = provider.Load();

		//	var configClient = new ConfigurationClient(mvcConfiguration.ServiceUrl);
		//	IConfiguration config = configClient.GetConfiguration();

		//	var cookieOptions = new CookieAuthenticationOptions
		//	{
		//		LoginPath = new PathString("/Authentication/Login"),
		//		CookieName = "SyringeOAuth"
		//	};

		//	app.UseCookieAuthentication(cookieOptions);

		//	// Only enable if there are credentials
		//	if (config.ContainsOAuthCredentials())
		//	{
		//		app.SetDefaultSignInAsAuthenticationType(cookieOptions.AuthenticationType);
		//	}

		//	//
		//	// OAuth2 Integrations
		//	//
		//	if (!string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientId) &&
		//		!string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientSecret))
		//	{
		//		// Console: https://console.developers.google.com/home/dashboard
		//		// Found under API and credentials.
		//		app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
		//		{
		//			ClientId = config.OAuthConfiguration.GoogleAuthClientId,
		//			ClientSecret = config.OAuthConfiguration.GoogleAuthClientSecret
		//		});
		//	}
		//	if (!string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientId) &&
		//		!string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientSecret))
		//	{
		//		// Console: https://account.live.com/developers/applications/
		//		// Make sure he 'redirecturl' is set to 'http://localhost:1980/Authentication/Noop' (or the domain being used), to match the CallbackPath
		//		app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
		//		{
		//			ClientId = config.OAuthConfiguration.MicrosoftAuthClientId,
		//			ClientSecret = config.OAuthConfiguration.MicrosoftAuthClientSecret,
		//			CallbackPath = new PathString("/Authentication/Noop")
		//		});
		//	}
		//	if (!string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientId) &&
		//		!string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientSecret))
		//	{
		//		// Console:  https://github.com/settings/developers
		//		// Set the callback url in the Github console to the same as the homepage url.
		//		var githubConfig = new GitHubAuthenticationOptions()
		//		{
		//			ClientId = config.OAuthConfiguration.GithubAuthClientId,
		//			ClientSecret = config.OAuthConfiguration.GithubAuthClientSecret,
		//		};

		//		if (config.OAuthConfiguration.ContainsGithubEnterpriseSettings())
		//		{
		//			githubConfig.Endpoints = new GitHubAuthenticationOptions.GitHubAuthenticationEndpoints()
		//			{
		//				AuthorizationEndpoint = config.OAuthConfiguration.GithubEnterpriseAuthorizationEndpoint,
		//				TokenEndpoint = config.OAuthConfiguration.GithubEnterpriseTokenEndpoint,
		//				UserInfoEndpoint = config.OAuthConfiguration.GithubEnterpriseUserInfoEndpoint
		//			};
		//		}

		//		app.UseGitHubAuthentication(githubConfig);
		//	}
		//}
	}
}
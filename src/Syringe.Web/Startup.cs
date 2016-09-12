using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Owin;
using Syringe.Web;
using Owin.Security.Providers.GitHub;
using StructureMap;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Web.App_Start;
using Syringe.Web.Configuration;
using Syringe.Web.DependencyResolution;

[assembly: OwinStartup(typeof(Startup))]

namespace Syringe.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ConfigureOAuth(app);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            // Call the service to get its configuration back
            var provider = new MvcConfigurationProvider(ConfigurationManager.AppSettings);
            MvcConfiguration mvcConfiguration = provider.Load();

            var configClient = new ConfigurationClient(mvcConfiguration.ServiceUrl);
            IConfiguration config = configClient.GetConfiguration();

            var cookieOptions = new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Authentication/Login"),
                CookieName = "SyringeOAuth"
            };

            app.UseCookieAuthentication(cookieOptions);

            // Only enable if there are credentials
            if (config.ContainsOAuthCredentials())
            {
                app.SetDefaultSignInAsAuthenticationType(cookieOptions.AuthenticationType);
            }


            //
            // OAuth2 Integrations
            //
            if (!string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientId) &&
                !string.IsNullOrEmpty(config.OAuthConfiguration.GoogleAuthClientSecret))
            {
                // Console: https://console.developers.google.com/home/dashboard
                // Found under API and credentials.
                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
                {
                    ClientId = config.OAuthConfiguration.GoogleAuthClientId,
                    ClientSecret = config.OAuthConfiguration.GoogleAuthClientSecret
                });
            }
            if (!string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientId) &&
                !string.IsNullOrEmpty(config.OAuthConfiguration.MicrosoftAuthClientSecret))
            {
                // Console: https://account.live.com/developers/applications/
                // Make sure he 'redirecturl' is set to 'http://localhost:1980/Authentication/Noop' (or the domain being used), to match the CallbackPath
                app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
                {
                    ClientId = config.OAuthConfiguration.MicrosoftAuthClientId,
                    ClientSecret = config.OAuthConfiguration.MicrosoftAuthClientSecret,
                    CallbackPath = new PathString("/Authentication/Noop")
                });
            }
            if (!string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientId) &&
                !string.IsNullOrEmpty(config.OAuthConfiguration.GithubAuthClientSecret))
            {
                // Console:  https://github.com/settings/developers
                // Set the callback url in the Github console to the same as the homepage url.
                var githubConfig = new GitHubAuthenticationOptions()
                {
                    ClientId = config.OAuthConfiguration.GithubAuthClientId,
                    ClientSecret = config.OAuthConfiguration.GithubAuthClientSecret,
                };

                if (config.OAuthConfiguration.ContainsGithubEnterpriseSettings())
                {
                    githubConfig.Endpoints = new GitHubAuthenticationOptions.GitHubAuthenticationEndpoints()
                    {
                        AuthorizationEndpoint = config.OAuthConfiguration.GithubEnterpriseAuthorizationEndpoint,
                        TokenEndpoint = config.OAuthConfiguration.GithubEnterpriseTokenEndpoint,
                        UserInfoEndpoint = config.OAuthConfiguration.GithubEnterpriseUserInfoEndpoint
                    };
                }

                app.UseGitHubAuthentication(githubConfig);
            }
        }
    }
}

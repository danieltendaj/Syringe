using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FakeN.Web;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Web.Controllers;

namespace Syringe.Tests.Unit.Web
{
	[TestFixture]
	public class AuthorizeWhenOAuthAttributeTests
	{
		private JsonConfiguration _config;

		[SetUp]
		public void Setup()
		{
			_config = new JsonConfiguration();
		}

		private AuthorizeWhenOAuthAttribute CreateAuthorizeWhenOAuthAttribute()
		{
			return new AuthorizeWhenOAuthAttribute(_config);
		}

		[Test]
		public void should_allow_anonymous_when_no_providers_set()
		{
			// given
			AuthorizeWhenOAuthAttribute attribute = CreateAuthorizeWhenOAuthAttribute();

			// when
			bool result = attribute.RunAuthorizeCore(new FakeHttpContext());

			// then
			Assert.True(result);
		}

		[Test]
		public void should_authorise_when_provider_set_and_user_is_authenticated()
		{
			// given
			_config.OAuthConfiguration.GithubAuthClientId = "github1";
			_config.OAuthConfiguration.GithubAuthClientSecret = "github2";
			AuthorizeWhenOAuthAttribute attribute = CreateAuthorizeWhenOAuthAttribute();

			var fakeHttpContext = new FakeHttpContext();
			((MutableIdentity) fakeHttpContext.User.Identity).IsAuthenticated = true;

			// when
			bool result = attribute.RunAuthorizeCore(fakeHttpContext);

			// then
			Assert.True(result);
		}
	}
}

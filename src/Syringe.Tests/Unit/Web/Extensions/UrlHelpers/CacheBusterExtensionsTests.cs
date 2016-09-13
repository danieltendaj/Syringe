using System;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Syringe.Web.Extensions.UrlHelpers;

namespace Syringe.Tests.Unit.Web.Extensions.UrlHelpers
{
    [TestFixture]
    public class CacheBusterExtensionsTests
    {
        private Func<UrlHelper, string, string> _resolvePathFunc;

        [SetUp]
        public void Setup()
        {
            _resolvePathFunc = CacheBusterExtensions.ResolvePath;
        }

        [TearDown]
        public void TearDown()
        {
            CacheBusterExtensions.ResolvePath = _resolvePathFunc;
        }

        [Test]
        public void should_return_expected_version_number()
        {
            // given
            const string expectedPath = "booya-beaches";
            string givenPath = null;
            CacheBusterExtensions.ResolvePath = (helper, s) => { givenPath = s; return expectedPath; };

            // when
            const string path = "yo-wuzzup";
            HtmlString result = CacheBusterExtensions.GetCacheBuster(null, path);

            // then
            string expectedVersion = $"{expectedPath}?v={CacheBusterExtensions.GetAssemblyVersion()}";
            Assert.That(result.ToString(), Is.EqualTo(expectedVersion));
            Assert.That(givenPath, Is.EqualTo(path));
        }
    }
}
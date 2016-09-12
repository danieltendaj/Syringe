using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Syringe.Web.Extensions.HtmlHelpers;

namespace Syringe.Tests.Unit.Web.Extensions.HtmlHelpers
{
    [TestFixture]
    public class CacheBusterExtensionsTests
    {
        private Func<string, string> _mapPathFunc;

        [SetUp]
        public void Setup()
        {
            _mapPathFunc = CacheBusterExtensions.MapServerPath;
        }

        [TearDown]
        public void TearDown()
        {
            CacheBusterExtensions.MapServerPath = _mapPathFunc;
        }

        [Test]
        public void should_return_expected_version_number()
        {
            // given
            const string expectedPath = "booya-beaches";
            string givenPath = null;
            CacheBusterExtensions.MapServerPath = s => { givenPath = s; return expectedPath; };

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
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
        [Test]
        public void should_return_expected_version_number()
        {
            // given

            // when
            HtmlString result = CacheBusterExtensions.GetCacheBuster(null);

            // then
            string expectedVersion = $"?v={CacheBusterExtensions.GetAssemblyVersion()}";
            Assert.That(result.ToString(), Is.EqualTo(expectedVersion));
        }
    }
}
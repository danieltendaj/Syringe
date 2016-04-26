using NUnit.Framework;
using Syringe.Core.Helpers;

namespace Syringe.Tests.Unit.Core.Helpers
{
    [TestFixture]
    public class UrlHelperTests
    {
        [Test]
        public void AddUrlBase_should_set_href_with_base_url()
        {
            var urlHelper = new UrlHelper();
            var addUrlBase = urlHelper.AddUrlBase("http://syringe.com/", content);

            Assert.AreEqual("<img src=\"http://syringe.com/images/test.jpg\"><a href=\"http://syringe.com/test\">Test</a>", addUrlBase);
        }

        [Test]
        public void AddUrlBase_should_add_trailing_forward_slash_to_base_url()
        {
            var urlHelper = new UrlHelper();
            var addUrlBase = urlHelper.AddUrlBase("http://syringe.com", content);

            Assert.AreEqual("<img src=\"http://syringe.com/images/test.jpg\"><a href=\"http://syringe.com/test\">Test</a>", addUrlBase);
        }

        [Test]
        [TestCase("http://syringe.com/test/test", "http://syringe.com/")]
        [TestCase("https://syringe.com/test/test", "https://syringe.com/")]
        public void GetBaseUrl_should_get_base_url_from_any_link_if_valid(string link, string expected)
        {
            var urlHelper = new UrlHelper();
            var addUrlBase = urlHelper.GetBaseUrl(link);

            Assert.AreEqual(expected, addUrlBase);
        }

        [Test]
        [TestCase("syringe.com/test/test", "syringe.com/test/test")]
        public void GetBaseUrl_should_return_same_url_if_it_cannot_be_parsed(string link, string expected)
        {
            var urlHelper = new UrlHelper();
            var addUrlBase = urlHelper.GetBaseUrl(link);

            Assert.AreEqual(expected, addUrlBase);
        }


        private string content = "<img src=\"/images/test.jpg\"><a href=\"/test\">Test</a>";
    }
}

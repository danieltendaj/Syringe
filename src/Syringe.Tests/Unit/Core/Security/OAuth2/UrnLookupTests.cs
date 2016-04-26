using Moq;
using NUnit.Framework;
using Syringe.Core.Security.OAuth2;

namespace Syringe.Tests.Unit.Core.Security.OAuth2
{
    [TestFixture]
    public class UrnLookupTests
    {
        [Test]
        public void GetNamespaceForId_should_return_correct_schema()
        {
            Assert.AreEqual("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", UrnLookup.GetNamespaceForId());
        }

        [Test]
        [TestCase("github")]
        [TestCase("GiTHuB")]
        public void GetNamespaceForName_should_return_github_namespace(string provider)
        {
            Assert.AreEqual("urn:github:name", UrnLookup.GetNamespaceForName(provider));
        }

        [Test]
        public void GetNamespaceForName_should_return_default_namespace()
        {
            Assert.AreEqual("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", UrnLookup.GetNamespaceForName(It.IsAny<string>()));
        }
    }
}

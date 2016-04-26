using System.Web;
using Moq;
using NUnit.Framework;
using Syringe.Core.Security;

namespace Syringe.Tests.Unit.Core.Security
{
    [TestFixture]
    public class UserContextTests
    {
        [Test]
        public void GetFromFormsAuth_should_return_user_not_logged_in_when_context_is_null()
        {
            var fromFormsAuth = UserContext.GetFromFormsAuth(null);

            Assert.AreEqual("Not Logged In", fromFormsAuth.FullName);
            Assert.AreEqual("anon", fromFormsAuth.Id);
        }

        [Test]
        public void GetFromFormsAuth_should_return_user_not_logged_in_when_formsCookie_is_null()
        {
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> requestMock = new Mock<HttpRequestBase>();
            requestMock.Setup(x => x.Cookies).Returns(new HttpCookieCollection());

            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            var fromFormsAuth = UserContext.GetFromFormsAuth(contextMock.Object);

            Assert.AreEqual("Not Logged In", fromFormsAuth.FullName);
            Assert.AreEqual("anon", fromFormsAuth.Id);
        }
    }
}

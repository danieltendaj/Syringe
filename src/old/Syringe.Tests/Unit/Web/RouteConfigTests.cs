using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using Syringe.Web;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class RouteConfigTests
    {
        [Test]
        public void should_register_default_route()
        {
            // given
            var collection = new RouteCollection();

            // when
            RouteConfig.RegisterRoutes(collection);

            // then
            Route instance = collection["Default"] as Route;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Url, Is.EqualTo("{controller}/{action}/{id}"));
            Assert.That(instance.Defaults["Controller"], Is.EqualTo("Home"));
            Assert.That(instance.Defaults["Action"], Is.EqualTo("Index"));
        }
    }
}
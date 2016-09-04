using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Syringe.Web;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class FilterConfigTests
    {
        [Test]
        public void should_register_log_filter()
        {
            // given
            var collection = new GlobalFilterCollection();

            // when
            FilterConfig.RegisterGlobalFilters(collection);

            // then
            var instance = collection.First().Instance;
            Assert.That(instance.GetType(), Is.EqualTo(typeof(LogExceptionsAttribute)));
        }
    }
}
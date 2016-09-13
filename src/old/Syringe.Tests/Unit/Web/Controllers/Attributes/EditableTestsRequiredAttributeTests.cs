using System.Web;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Web.Controllers.Attribute;

namespace Syringe.Tests.Unit.Web.Controllers.Attributes
{
    [TestFixture]
    public class EditableTestsRequiredAttributeTests
    {
        [Test]
        public void should_throw_exception_when_running_in_read_only_mode()
        {
            // given
            var attribute = new EditableTestsRequiredAttribute
            {
                Configuration = new JsonConfiguration
                {
                    ReadonlyMode = true
                }
            };

            // when & then
            Assert.Throws<HttpException>(() => attribute.OnActionExecuting(null));
        }

        [Test]
        public void should_not_throw_exception_when_not_running_in_read_only_mode()
        {
            // given
            var attribute = new EditableTestsRequiredAttribute
            {
                Configuration = new JsonConfiguration
                {
                    ReadonlyMode = false
                }
            };

            // when & then
            attribute.OnActionExecuting(null);
        }
    }
}
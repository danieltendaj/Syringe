using NUnit.Framework;
using Syringe.Web.ValidationAttributes;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class ValidCsQueryAttributeTests
    {
        [Test]
        public void IsValid_should_return_false_when_value_is_null()
        {
            Assert.IsFalse(new ValidCsQueryAttribute().IsValid(null));
        }

        [Test]
        public void IsValid_should_return_false_when_selector_is_invalid()
        {
            Assert.IsFalse(new ValidCsQueryAttribute().IsValid("["));
        }

        [Test]
        [TestCase("syringe > you")]
        [TestCase("i.like.to.pee")]
        public void IsValid_should_return_true_when_selector_is_valid(string selector)
        {
            Assert.IsTrue(new ValidCsQueryAttribute().IsValid(selector));
        }
    }
}

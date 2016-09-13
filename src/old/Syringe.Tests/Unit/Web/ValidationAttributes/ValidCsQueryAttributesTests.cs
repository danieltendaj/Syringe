using NUnit.Framework;
using Syringe.Web.Models.ValidationAttributes;

namespace Syringe.Tests.Unit.Web.ValidationAttributes
{
    [TestFixture]
    public class ValidCsQueryAttributeTests
    {
        [Test]
        public void IsValid_should_return_false_when_value_is_null()
        {
            Assert.IsFalse(new ValidCssSelectorAttribute().IsValid(null));
        }

        [Test]
        public void IsValid_should_return_false_when_selector_is_invalid()
        {
            Assert.IsFalse(new ValidCssSelectorAttribute().IsValid("["));
        }

        [Test]
        [TestCase("syringe > you")]
        [TestCase("i.like.to.pee")]
        public void IsValid_should_return_true_when_selector_is_valid(string selector)
        {
            Assert.IsTrue(new ValidCssSelectorAttribute().IsValid(selector));
        }
    }
}

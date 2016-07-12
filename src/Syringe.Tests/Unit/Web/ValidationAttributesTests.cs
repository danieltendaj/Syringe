using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using Syringe.Web.ValidationAttributes;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class ValidRegexAttributeTests
    {
        [Test]
        public void IsValid_should_return_false_when_value_is_null()
        {
            Assert.IsFalse(new ValidRegexAttribute().IsValid(null));
        }

        [Test]
        public void IsValid_should_return_false_when_regex_is_invalid()
        {
            Assert.IsFalse(new ValidRegexAttribute().IsValid("["));
        }

        [Test]
        [TestCase("(.*)+")]
        [TestCase(@"[\w]+")]
        public void IsValid_should_return_true_when_regex_is_valid(string regex)
        {
            Assert.IsTrue(new ValidRegexAttribute().IsValid(regex));
        }
    }
}

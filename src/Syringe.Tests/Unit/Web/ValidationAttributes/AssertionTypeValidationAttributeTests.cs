using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using Syringe.Web.Models;
using Syringe.Web.Models.ValidationAttributes;

namespace Syringe.Tests.Unit.Web.ValidationAttributes
{
    [TestFixture]
    public class AssertionTypeValidationAttributeTests : AssertionTypeValidationAttribute
    {
        [Test]
        public void IsValid_should_return_null_when_AssertionTypeValidation_value_is_null()
        {
            //given
            AssertionViewModel model = new AssertionViewModel
            {
                Description = string.Empty,
                Value = string.Empty
            };

            // when
            var validationResult = IsValid(null, new ValidationContext(model));

            // then
            Assert.Null(validationResult);
        }
    }
}
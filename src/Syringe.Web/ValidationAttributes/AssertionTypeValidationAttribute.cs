using System.ComponentModel.DataAnnotations;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Web.ValidationAttributes
{
    public class AssertionTypeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var model = (AssertionViewModel)context.ObjectInstance;

            if (model.AssertionMethod == AssertionMethod.CSQuery)
            {
                ValidCsQueryAttribute attribute = new ValidCsQueryAttribute { ErrorMessage = "Invalid CSS Selector" };
                return attribute.GetValidationResult(value, context);
            }
            if (model.AssertionMethod == AssertionMethod.Regex)
            {
                ValidRegexAttribute attribute = new ValidRegexAttribute { ErrorMessage = "Invalid Regex" };
                return attribute.GetValidationResult(value, context);
            }

            //no assertion type so always valid
            return ValidationResult.Success;
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Syringe.Core.Tests;

namespace Syringe.Web.Models.ValidationAttributes
{
    public class AssertionTypeValidationAttribute : ValidationAttribute
    {
        private static readonly Regex _variableReplacer = new Regex("{.+}", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var model = (AssertionViewModel)context.ObjectInstance;

            if (value == null)
                return new ValidationResult("This field is mandatory");

            // replace variables so they don't interfere with validation
            value = ReplaceVariables(value.ToString());

            if (model.AssertionMethod == AssertionMethod.CssSelector)
            {
                var attribute = new ValidCssSelectorAttribute { ErrorMessage = "Invalid CSS Selector" };
                return attribute.GetValidationResult(value, context);
            }
            if (model.AssertionMethod == AssertionMethod.Regex)
            {
                var attribute = new ValidRegexAttribute { ErrorMessage = "Invalid Regex" };
                return attribute.GetValidationResult(value, context);
            }

            //no assertion type so always valid
            return ValidationResult.Success;
        }


        private static string ReplaceVariables(string value)
        {
            return _variableReplacer.Replace(value, string.Empty);
        }
    }
}
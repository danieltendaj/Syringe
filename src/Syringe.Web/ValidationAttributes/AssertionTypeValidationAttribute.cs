using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Web.ValidationAttributes
{
    public class AssertionTypeValidationAttribute : ValidationAttribute
    {
        private static readonly Regex _variableReplacer = new Regex("{.+}", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var model = (AssertionViewModel)context.ObjectInstance;

            // replace variables so they don't interfere with validation
            value = ReplaceVariables(value.ToString());

            if (model.AssertionMethod == AssertionMethod.CSQuery)
            {
                var attribute = new ValidCsQueryAttribute { ErrorMessage = "Invalid CSS Selector" };
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
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Syringe.Web.ValidationAttributes
{
    public class ValidRegexAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            try
            {
                string regex = value.ToString();
                Regex.Match("", regex);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
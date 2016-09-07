using System.ComponentModel.DataAnnotations;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace Syringe.Web.Models.ValidationAttributes
{
    public class ValidCssSelectorAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            try
            {
                var parser = new HtmlParser();
                IHtmlDocument document = parser.Parse("<html></html>");
                document.QuerySelector(value.ToString());

                return true;
            }
            catch (DomException)
            {
                return false;
            }
        }
    }
}
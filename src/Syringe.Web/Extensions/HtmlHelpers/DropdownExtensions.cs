using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Syringe.Web.Extensions.HtmlHelpers
{
    public static class DropdownExtensions
    {
        public static MvcHtmlString GenerateHttpStatusDropdown<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var items = new List<SelectListItem>();
            foreach (string name in Enum.GetNames(typeof(HttpStatusCode)))
            {
                string value = Convert.ToInt32(Enum.Parse(typeof(HttpStatusCode), name)).ToString();
                items.Add(new SelectListItem { Text = $"{name} ({value})", Value = value });
            }

            var selectList = new SelectList(items, "Value", "Text");

            return htmlHelper.DropDownListFor(expression, selectList, null, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

		public static MvcHtmlString GenerateScriptSnippetsDropdown<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, string>> propertyValueExpression, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
			where TProperty: IEnumerable<string>
            
		{
			var items = new List<SelectListItem>();
			items.Add(new SelectListItem { Text = "None", Value = "" });

			Func<TModel, TProperty> call = expression.Compile();
			IEnumerable<string> snippetItems = call(htmlHelper.ViewData.Model);

		    Func<TModel, string> propValCall = propertyValueExpression.Compile();
            string propertyValue = propValCall(htmlHelper.ViewData.Model);

		    if (snippetItems != null)
		    {
		        foreach (var item in snippetItems)
		        {
		            items.Add(new SelectListItem {Text = $"{item}", Value = item});
		        }
		    }

		    var selectList = new SelectList(items, "Value", "Text", propertyValue);
			//htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return htmlHelper.DropDownListFor(propertyValueExpression, selectList, null, htmlAttributes);
		}
	}
}
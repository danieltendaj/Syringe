using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Syringe.Web.Extensions
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
    }
}
using System;
using System.Collections;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Web.Extensions;
using Syringe.Web.Extensions.HtmlHelpers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void MinutesAndSecondsFormat_should_return_time_in_milliseconds_if_less_then_a_second()
        {
            // given + when
            var minutesAndSecondsFormat = new TimeSpan(0, 0, 0, 0, 10).MinutesAndSecondsFormat();

            // then
            Assert.AreEqual("10 milliseconds", minutesAndSecondsFormat);
        }

        [Test]
        public void MinutesAndSecondsFormat_should_return_time_in_seconds_if_less_then_a_minute()
        {
            // given + when
            var minutesAndSecondsFormat = new TimeSpan(0, 0, 0, 58).MinutesAndSecondsFormat();

            // then
            Assert.AreEqual("58 seconds", minutesAndSecondsFormat);
        }

        [Test]
        public void MinutesAndSecondsFormat_should_return_time_in_minutes_and_seconds_if_more_then_a_minute()
        {
            // given + when
            var minutesAndSecondsFormat = new TimeSpan(0, 0, 1, 58).MinutesAndSecondsFormat();

            // then
            Assert.AreEqual("1m 58s", minutesAndSecondsFormat);
        }

	    [Test]
	    public void GenerateScriptSnippetsDropdown_should_generate_dropdown_list_with_items()
	    {
			// given (some ridiculous amount of setup for a HtmlHelper extension method)
			var helper = MvcHelper.GetHtmlHelper<TestViewModel>(new TestViewModel());
		    var testViewModel = new TestViewModel();
			testViewModel.BeforeExecuteScriptSnippets = new string[]
			{
				"snippet1.snippet",
				"snippet2.snippet",
				"snippet3.snippet"
			};
		    testViewModel.BeforeExecuteScriptFilename = "snippet2.snippet";

		    helper.ViewData.Model = testViewModel;

		    string expectedHtml =
			    @"<select class=""myclass"" id=""BeforeExecuteScriptFilename"" name=""BeforeExecuteScriptFilename""><option value="""">None</option>
<option value=""snippet1.snippet"">snippet1.snippet</option>
<option selected=""selected"" value=""snippet2.snippet"">snippet2.snippet</option>
<option value=""snippet3.snippet"">snippet3.snippet</option>
</select>";

			expectedHtml = expectedHtml.Replace("\n", "").Replace("\r", "");

			// when
			MvcHtmlString htmlString = helper.GenerateScriptSnippetsDropdown(
				m => m.BeforeExecuteScriptFilename, 
				m => m.BeforeExecuteScriptSnippets,
			    new {@class = "myclass"});

			// then
		    string actualHtml = htmlString.ToHtmlString();
		    Assert.That(actualHtml, Is.Not.Null.Or.Empty);

		    actualHtml = actualHtml.Replace("\n", "").Replace("\r", "");
			Assert.That(actualHtml, Contains.Substring(expectedHtml));
	    }
    }

	public static class MvcHelper
	{
		public static HtmlHelper<TModel> GetHtmlHelper<TModel>(TModel inputDictionary)
		{
			var viewData = new ViewDataDictionary<TModel>(inputDictionary);
			var mockViewContext = new Mock<ViewContext> { CallBase = true };
			mockViewContext.Setup(c => c.ViewData).Returns(viewData);
			mockViewContext.Setup(c => c.HttpContext.Items).Returns(new Hashtable());

			return new HtmlHelper<TModel>(mockViewContext.Object, GetViewDataContainer(viewData));
		}

		public static IViewDataContainer GetViewDataContainer(ViewDataDictionary viewData)
		{
			var mockContainer = new Mock<IViewDataContainer>();
			mockContainer.Setup(c => c.ViewData).Returns(viewData);
			return mockContainer.Object;
		}
	}
}

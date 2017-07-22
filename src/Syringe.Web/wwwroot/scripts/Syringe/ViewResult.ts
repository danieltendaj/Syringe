/// <reference path="../typings/jquery/jquery.d.ts" />

module Syringe.Web
{
	export class ViewResult
	{
		constructor()
		{
			this.init();
		}

		private init()
		{
			$("#tests-passed-count").on("click", () =>
			{
				$.each($(".test-failed"), (count, item) =>
				{
					$(item).hide(250);
				});

				$.each($(".test-passed"), (count, item) =>
				{
					$(item).show(400);
				});
				
            });

            $("#all-tests-count").on("click", () =>
			{
                $.each($(".test-failed, .test-passed"), (count, item) =>
				{
					$(item).show(400);
				});

			});

			$("#tests-failed-count").on("click", () =>
			{
				$.each($(".test-passed"), (count, item) =>
				{
					$(item).hide(250);
				});

				$.each($(".test-failed"), (count, item) =>
				{
					$(item).show(400);
				});
			});
		}
	}
}

new Syringe.Web.ViewResult();
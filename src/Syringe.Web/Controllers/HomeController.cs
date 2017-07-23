using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Controllers.Attribute;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[AuthorizeWhenOAuth]
	public class HomeController : Controller
	{
		private readonly ITestService _testsClient;
		private readonly Func<IRunViewModel> _runViewModelFactory;
		private readonly IHealthCheck _healthCheck;
		private readonly IEnvironmentsService _environmentsService;
		private readonly IConfiguration _configuration;

		public HomeController(
			ITestService testsClient,
			Func<IRunViewModel> runViewModelFactory,
			IHealthCheck healthCheck,
			IEnvironmentsService environmentsService,
			IConfiguration configuration)
		{
			_testsClient = testsClient;
			_runViewModelFactory = runViewModelFactory;
			_healthCheck = healthCheck;
			_environmentsService = environmentsService;
			_configuration = configuration;
		}

		public ActionResult Index(int pageNumber = 1, int noOfResults = 10)
		{
			RunHealthChecks();
			ViewBag.Title = "All test files";

			IEnumerable<string> files = _testsClient.ListFiles().ToList();

			var model = new IndexViewModel
			{
				PageNumber = pageNumber,
				NoOfResults = noOfResults,
				PageNumbers = files.GetPageNumbersToShow(noOfResults),
				Files = files.GetPaged(noOfResults, pageNumber),
				Environments = _environmentsService.Get().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray()
			};

			string viewName = "Index";
			if (_configuration.ReadonlyMode)
			{
				viewName = "Index-ReadonlyMode";
			}

			return View(viewName, model);
		}

		[HttpPost]
		public ActionResult Run(string filename, string environment)
		{
			// todo: NETStandard
			//UserContext context = UserContext.GetFromFormsAuth(HttpContext);
			UserContext context = new UserContext()
			{
				FullName = "TODO",
				Id = "TODO",
				IsGuest = false
			};

			var runViewModel = _runViewModelFactory();
			runViewModel.Run(context, filename, environment);
			return View("Run", runViewModel);
		}

		private void RunHealthChecks()
		{
			_healthCheck.CheckServiceConfiguration();
			_healthCheck.CheckServiceSwaggerIsRunning();
		}
	}
}
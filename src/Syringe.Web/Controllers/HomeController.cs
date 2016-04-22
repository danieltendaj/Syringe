using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[Authorize]
    public class HomeController : Controller
    {
        private readonly ITestService _testsClient;
        private readonly Func<IRunViewModel> _runViewModelFactory;
		private readonly IHealthCheck _healthCheck;
	    private readonly IEnvironmentsService _environmentsService;

	    public HomeController(
            ITestService testsClient,
            Func<IRunViewModel> runViewModelFactory,
			IHealthCheck healthCheck,
            IEnvironmentsService environmentsService)
        {
            _testsClient = testsClient;
            _runViewModelFactory = runViewModelFactory;
			_healthCheck = healthCheck;
	        _environmentsService = environmentsService;
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
                Environments = _environmentsService.List().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray()
            };

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Run(string filename, string environment)
        {
			UserContext context = UserContext.GetFromFormsAuth(HttpContext);

			var runViewModel = _runViewModelFactory();
            runViewModel.Run(context, filename, environment);
            return View("Run", runViewModel);
        }
        
        [HttpPost]
        public ActionResult RunTest(string filename, int position, string environment)
        {
            UserContext context = UserContext.GetFromFormsAuth(HttpContext);

            var runViewModel = _runViewModelFactory();
            runViewModel.RunTest(context, filename, environment, position);
            return View("Run", runViewModel);
        }

        private void RunHealthChecks()
        {
			_healthCheck.CheckServiceConfiguration();
			_healthCheck.CheckServiceSwaggerIsRunning();
        }
	}
}
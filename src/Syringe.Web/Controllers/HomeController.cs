using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Core.Helpers;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests.Results;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[Authorize]
    public class HomeController : Controller
    {
        private readonly ITestService _testsClient;
        private readonly IUserContext _userContext;
        private readonly Func<IRunViewModel> _runViewModelFactory;
		private readonly IHealthCheck _healthCheck;
	    private readonly IUrlHelper _urlHelper;
	    private readonly IEnvironmentsService _environmentsService;

	    public HomeController(
            ITestService testsClient,
            IUserContext userContext,
            Func<IRunViewModel> runViewModelFactory,
			IHealthCheck healthCheck, 
            IUrlHelper urlHelper,
            IEnvironmentsService environmentsService)
        {
            _testsClient = testsClient;
            _userContext = userContext;
            _runViewModelFactory = runViewModelFactory;
			_healthCheck = healthCheck;
	        _urlHelper = urlHelper;
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

        public ActionResult AllResults()
        {
            return View("AllResults", _testsClient.GetSummaries());
        }

        public ActionResult TodaysResults()
        {
            return View("AllResults", _testsClient.GetSummariesForToday());
        }

        public ActionResult ViewResult(Guid id)
        {
            return View("ViewResult", _testsClient.GetResultById(id));
        }

		[HttpPost]
        public async Task<ActionResult> DeleteResult(Guid id)
        {
            TestFileResult session = _testsClient.GetResultById(id);
            await _testsClient.DeleteResultAsync(session.Id);

            return RedirectToAction("AllResults");
        }

		public ActionResult ViewHtml(Guid testFileResultId, int resultId)
		{
			TestFileResult testFileResult = _testsClient.GetResultById(testFileResultId);
		    TestResult result = testFileResult.TestResults.ElementAtOrDefault(resultId);
			if (result != null)
			{
				string html = result.HttpContent;
				string baseUrl = _urlHelper.GetBaseUrl(result.ActualUrl);
				html = _urlHelper.AddUrlBase(baseUrl, html);

				return Content(html);
			}

			return Content("Result Id not found");
		}

		public ActionResult ViewHttpLog(Guid testFileResultId, int resultId)
		{
			TestFileResult testFileResult = _testsClient.GetResultById(testFileResultId);
			TestResult result = testFileResult.TestResults.ElementAtOrDefault(resultId);
            if (result != null)
				return Content(result.HttpLog, "text/plain");

			return Content("Result Id not found");
		}

		public ActionResult ViewLog(Guid testFileResultId, int resultId)
		{
			TestFileResult testFileResult = _testsClient.GetResultById(testFileResultId);
			TestResult result = testFileResult.TestResults.ElementAtOrDefault(resultId);
            if (result != null)
				return Content(result.Log, "text/plain");

			return Content("Result Id not found");
		}
	}
}
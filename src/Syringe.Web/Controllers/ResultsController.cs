using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Syringe.Core.Helpers;
using Syringe.Core.Services;
using Syringe.Core.Tests.Results;

namespace Syringe.Web.Controllers
{
    [AuthorizeWhenOAuth]
    public class ResultsController : Controller
    {
        private readonly IUrlHelper _urlHelper;
        private readonly ITestService _testsClient;
        private readonly IEnvironmentsService _environmentsService;

        public ResultsController(IUrlHelper urlHelper, ITestService testsClient, IEnvironmentsService environmentsService)
        {
            _urlHelper = urlHelper;
            _testsClient = testsClient;
            _environmentsService = environmentsService;
        }
        
        public async Task<ActionResult> Index(int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            ViewBag.Title =  string.IsNullOrEmpty(environment) ? "All" : environment;

            TestFileResultSummaryCollection result = await _testsClient.GetSummaries(DateTime.Today.AddYears(-1), pageNumber, noOfResults, environment);
            result.Environments = _environmentsService.Get().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray();
            result.Environment = environment;
            return View("Index", result);
        }

        public async Task<ActionResult> Today(int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            ViewBag.Title = string.IsNullOrEmpty(environment) ? "All" : environment;

            TestFileResultSummaryCollection result = await _testsClient.GetSummaries(DateTime.Today, pageNumber, noOfResults, environment);
            result.Environments = _environmentsService.Get().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray();
            result.Environment = environment;
            return View("Index", result);
        }

        public ActionResult ViewResult(Guid id)
        {
            return View("ViewResult", _testsClient.GetResultById(id));
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            TestFileResult session = _testsClient.GetResultById(id);
            _testsClient.DeleteResult(session.Id);

            return RedirectToAction("Index");
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
            {
                return Content(result.HttpLog, "text/plain");
            }

            return Content("Result Id not found");
        }

        public ActionResult ViewLog(Guid testFileResultId, int resultId)
        {
            TestFileResult testFileResult = _testsClient.GetResultById(testFileResultId);
            TestResult result = testFileResult.TestResults.ElementAtOrDefault(resultId);
            if (result != null)
            {
                return Content(result.Log, "text/plain");
            }

            return Content("Result Id not found");
        }
    }
}
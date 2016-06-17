using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Syringe.Core.Helpers;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests.Results;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    [AuthorizeWhenOAuth]
    public class ResultsController : Controller
    {
        private readonly ITasksService _tasksClient;
        private readonly IUrlHelper _urlHelper;
        private readonly ITestService _testsClient;
        private readonly IEnvironmentsService _environmentsService;

        public ResultsController(ITasksService tasksClient, IUrlHelper urlHelper, ITestService testsClient, IEnvironmentsService environmentsService)
        {
            _tasksClient = tasksClient;
            _urlHelper = urlHelper;
            _testsClient = testsClient;
            _environmentsService = environmentsService;
        }

        public ActionResult Html(int taskId, int position)
        {
            TestResult testResult = FindTestResult(taskId, position);

            if (testResult == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified test.");
            }

            var baseUrl = _urlHelper.GetBaseUrl(testResult.ActualUrl);

            var viewModel = new ResultsViewModel
            {
                ActualUrl = testResult.ActualUrl,
                Content = testResult.HttpResponse == null ? string.Empty : _urlHelper.AddUrlBase(baseUrl, testResult.HttpResponse.Content)
            };

            return View(viewModel);
        }

        private TestResult FindTestResult(int taskId, int position)
        {
            TaskDetails taskDetails = _tasksClient.GetRunningTaskDetails(taskId);
            TestResult task = taskDetails.Results.ElementAtOrDefault(position);

            return task;
        }

        public ActionResult Raw(int taskId, int position)
        {
            TestResult testResult = FindTestResult(taskId, position);

            if (testResult == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified test.");
            }

            var viewModel = new ResultsViewModel
            {
                ActualUrl = testResult.ActualUrl,
                Content = testResult.HttpResponse == null ? string.Empty : testResult.HttpResponse.Content
            };

            return View(viewModel);
        }

        public async Task<ActionResult> Index(int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            ViewBag.Title = "All results";

            TestFileResultSummaryCollection result = await _testsClient.GetSummaries(DateTime.Today.AddYears(-1), pageNumber, noOfResults, environment);
            result.Environments = _environmentsService.List().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray();
            result.Environment = environment;
            return View("Index", result);
        }

        public async Task<ActionResult> Today(int pageNumber = 1, int noOfResults = 20, string environment = "")
        {
            ViewBag.Title = "Today's results";

            TestFileResultSummaryCollection result = await _testsClient.GetSummaries(DateTime.Today, pageNumber, noOfResults, environment);
            result.Environments = _environmentsService.List().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray();
            result.Environment = environment;
            return View("Index", result);
        }

        public ActionResult ViewResult(Guid id)
        {
            return View("ViewResult", _testsClient.GetResultById(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            TestFileResult session = _testsClient.GetResultById(id);
            await _testsClient.DeleteResultAsync(session.Id);

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
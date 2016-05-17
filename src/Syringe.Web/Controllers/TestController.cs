using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Web.Mappers;
using Syringe.Web.Models;


namespace Syringe.Web.Controllers
{
	[AuthorizeWhenOAuth]
	public class TestController : Controller
	{
		private readonly ITestService _testsClient;
		private readonly ITestFileMapper _testFileMapper;
	    private readonly IEnvironmentsService _environmentsService;
		private readonly IConfiguration _configuration;

		public TestController(
			ITestService testsClient,
			ITestFileMapper testFileMapper,
            IEnvironmentsService environmentsService,
			IConfiguration configuration)
		{
			_testsClient = testsClient;
			_testFileMapper = testFileMapper;
	        _environmentsService = environmentsService;
			_configuration = configuration;
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			AddPagingDataForBreadCrumb();
			base.OnActionExecuting(filterContext);
		}

		public ActionResult View(string filename, int pageNumber = 1, int noOfResults = 10)
		{
            TestFile testFile = _testsClient.GetTestFile(filename);
			IEnumerable<Test> tests = testFile.Tests.GetPaged(noOfResults, pageNumber);

			var viewModel = new TestFileViewModel
			{
				PageNumbers = testFile.Tests.GetPageNumbersToShow(noOfResults),
				Tests = _testFileMapper.BuildTests(tests),
				Filename = filename,
				PageNumber = pageNumber,
				NoOfResults = noOfResults,
                Environments = _environmentsService.List().OrderBy(x => x.Order).ThenBy(x => x.Name).Select(x => x.Name).ToArray()
			};

			string viewName = "View";
			if (_configuration.ReadonlyMode)
			{
				viewName = "View-ReadonlyMode";
			}

			return View(viewName, viewModel);
		}

        [HttpGet]
		[EditableTestsRequired]
		public ActionResult Edit(string filename, int position)
		{
            Test test = _testsClient.GetTest(filename, position);
			TestViewModel model = _testFileMapper.BuildViewModel(test);

			return View("Edit", model);
		}

		[HttpPost]
		[ValidateInput(false)]
		[EditableTestsRequired]
		public ActionResult Edit(TestViewModel model)
		{
			if (ModelState.IsValid)
			{
				Test test = _testFileMapper.BuildCoreModel(model);
                _testsClient.EditTest(test);
				return RedirectToAction("View", new { filename = model.Filename });
			}

			return View("Edit", model);
		}

        [HttpGet]
		[EditableTestsRequired]
		public ActionResult Add(string filename)
		{
            TestFile testFile = _testsClient.GetTestFile(filename);
            var model = new TestViewModel
            {
                Filename = filename,
                AvailableVariables = _testFileMapper.BuildVariableViewModel(testFile),
                Method = MethodType.GET,
                ExpectedHttpStatusCode = HttpStatusCode.OK
            };

			return View("Edit", model);
		}

		[HttpPost]
		[ValidateInput(false)]
		[EditableTestsRequired]
		public ActionResult Add(TestViewModel model)
		{
			if (ModelState.IsValid)
			{
				Test test = _testFileMapper.BuildCoreModel(model);
                _testsClient.CreateTest(test);
				return RedirectToAction("View", new { filename = model.Filename });
			}

			return View("Edit", model);
		}

		[HttpPost]
		[EditableTestsRequired]
		public ActionResult Delete(int position, string fileName)
		{
            _testsClient.DeleteTest(position, fileName);

			return RedirectToAction("View", new { filename = fileName });
		}

	    [HttpPost]
		[EditableTestsRequired]
		public ActionResult Copy(int position, string fileName)
        {
            _testsClient.CopyTest(position, fileName);

            return RedirectToAction("View", new { filename = fileName });
        }

		[EditableTestsRequired]
		public ActionResult AddAssertion()
		{
			return PartialView("EditorTemplates/AssertionViewModel", new AssertionViewModel());
		}

		[EditableTestsRequired]
		public ActionResult AddCapturedVariableItem()
		{
			return PartialView("EditorTemplates/CapturedVariableItem", new CapturedVariableItem());
		}

		[EditableTestsRequired]
		public ActionResult AddHeaderItem()
		{
			return PartialView("EditorTemplates/HeaderItem", new Models.HeaderItem());
		}

		public ActionResult ViewRawFile(string fileName)
		{
            var model = new TestFileViewModel { Filename = fileName, RawFile = _testsClient.GetRawFile(fileName) };
			return View("ViewRawFile", model);
		}

		private void AddPagingDataForBreadCrumb()
		{
			// Paging support for the breadcrumb trail
			ViewBag.PageNumber = Request.QueryString["pageNumber"];
			ViewBag.NoOfResults = Request.QueryString["noOfResults"];
		}
	}
}
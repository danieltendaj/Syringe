using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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

		public TestController(
			ITestService testsClient,
			ITestFileMapper testFileMapper,
            IEnvironmentsService environmentsService)
		{
			_testsClient = testsClient;
			_testFileMapper = testFileMapper;
	        _environmentsService = environmentsService;
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

			return View("View", viewModel);
		}

		public ActionResult Edit(string filename, int position)
		{
            Test test = _testsClient.GetTest(filename, position);
			TestViewModel model = _testFileMapper.BuildViewModel(test);

			return View("Edit", model);
		}

		[HttpPost]
		[ValidateInput(false)]
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

		public ActionResult Add(string filename)
		{
            TestFile testFile = _testsClient.GetTestFile(filename);

            var model = new TestViewModel { Filename = filename, AvailableVariables = _testFileMapper.BuildVariableViewModel(testFile) };
			return View("Edit", model);
		}

		[HttpPost]
		[ValidateInput(false)]
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
		public ActionResult Delete(int position, string fileName)
		{
            _testsClient.DeleteTest(position, fileName);

			return RedirectToAction("View", new { filename = fileName });
		}

	    [HttpPost]
	    public ActionResult Copy(TestViewModel model)
	    {
	        return null;
	    }

		public ActionResult AddAssertion()
		{
			return PartialView("EditorTemplates/AssertionViewModel", new AssertionViewModel());
		}

		public ActionResult AddCapturedVariableItem()
		{
			return PartialView("EditorTemplates/CapturedVariableItem", new CapturedVariableItem());
		}

		public ActionResult AddHeaderItem()
		{
			return PartialView("EditorTemplates/HeaderItem", new Models.HeaderItem());
		}

		private void AddPagingDataForBreadCrumb()
		{
			// Paging support for the breadcrumb trail
			ViewBag.PageNumber = Request.QueryString["pageNumber"];
			ViewBag.NoOfResults = Request.QueryString["noOfResults"];
		}

		public ActionResult ViewRawFile(string fileName)
		{
            var model = new TestFileViewModel { Filename = fileName, RawFile = _testsClient.GetRawFile(fileName) };
			return View("ViewRawFile", model);
		}
	}
}
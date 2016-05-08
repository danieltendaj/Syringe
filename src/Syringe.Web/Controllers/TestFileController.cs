using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[AuthorizeWhenOAuth]
	public class TestFileController : Controller
    {
        private readonly ITestService _testsClient;
        private readonly IUserContext _userContext;
        private readonly IEnvironmentsService _environmentsService;

        public TestFileController(ITestService testsClient, IUserContext userContext, IEnvironmentsService environmentsService)
        {
            _testsClient = testsClient;
            _userContext = userContext;
            _environmentsService = environmentsService;
        }

        public ActionResult Add()
        {
            var model = new TestFileViewModel();
            return View("Add", model);
        }

        [HttpPost]
		[ValidateInput(false)]
        public ActionResult Add(TestFileViewModel model)
        {
            SelectListItem[] environments = GetEnvironmentsDropDown();

            if (ModelState.IsValid)
            {
                var testFile = new TestFile
                {
                    Filename = model.Filename,
                    Variables = model.Variables?.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToList() ?? new List<Variable>()
                };

                bool createdTestFile = _testsClient.CreateTestFile(testFile);
                if (createdTestFile)
                {
                    return RedirectToAction("Index", "Home");
            }
            }

            if (model.Variables != null)
            {
                foreach (VariableViewModel variable in model.Variables)
                {
                    variable.AvailableEnvironments = environments;
                }
            }

            return View("Add", model);
        }

        public ActionResult Update(string fileName)
        {
            TestFile testFile = _testsClient.GetTestFile(fileName);
            SelectListItem[] environments = GetEnvironmentsDropDown();

            var variables = testFile.Variables
                .Select(x => new VariableViewModel
                {
                    Name = x.Name,
                    Value = x.Value,
                    Environment = x.Environment.Name,
                    AvailableEnvironments = environments
                })
                .ToList();

            var model = new TestFileViewModel
            {
                Filename = fileName,
                Variables = variables
            };

            return View("Update", model);
        }

        [HttpPost]
        public ActionResult Delete(string filename)
        {
            _testsClient.DeleteFile(filename);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
		[ValidateInput(false)]
        public ActionResult Update(TestFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var testFile = new TestFile
                {
                    Filename = model.Filename,
                    Variables = model.Variables?.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToList() ?? new List<Variable>()
				};

                bool updateTestFile = _testsClient.UpdateTestVariables(testFile);
                if (updateTestFile)
                {
                    return RedirectToAction("Index", "Home");
            }
            }

            return View("Update", model);
        }

        public ActionResult AddVariableItem()
        {
            var model = new VariableViewModel
            {
                AvailableEnvironments = GetEnvironmentsDropDown()
            };

            return PartialView("EditorTemplates/VariableViewModel", model);
        }

        private SelectListItem[] GetEnvironmentsDropDown()
        {
            var environments = _environmentsService.List();

            return environments
                .OrderBy(x => x.Order)
                .Select(x => new SelectListItem { Value = x.Name, Text = x.Name })
                .ToArray();
        }
	}
}
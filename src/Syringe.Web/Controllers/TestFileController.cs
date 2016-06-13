using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;
using Syringe.Web.Models;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Web.Controllers
{
    [AuthorizeWhenOAuth]
    public class TestFileController : Controller
    {
        private readonly ITestService _testsClient;
        private readonly IEnvironmentsService _environmentsService;
        private const string DEFAULT_ENV_TEXT = "--[[Default Environment]]--";

        public TestFileController(ITestService testsClient, IEnvironmentsService environmentsService)
        {
            _testsClient = testsClient;
            _environmentsService = environmentsService;
        }

        [HttpGet]
        [EditableTestsRequired]
        public ActionResult Add()
        {
            var model = new TestFileViewModel();
            return View("Add", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [EditableTestsRequired]
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

        [HttpGet]
        [EditableTestsRequired]
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
        [EditableTestsRequired]
        public ActionResult Delete(string filename)
        {
            _testsClient.DeleteFile(filename);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateInput(false)]
        [EditableTestsRequired]
        public ActionResult Update(TestFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var variables = new List<Variable>(model.Variables.Count);
                foreach (var variableModel in model.Variables)
                {
                    //Variables = model.Variables?.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToList() ?? new List<Variable>()
                    string environment = variableModel.Environment == DEFAULT_ENV_TEXT ? string.Empty : variableModel.Environment;
                    variables.Add(new Variable(variableModel.Name, variableModel.Value, environment));
                }

                var testFile = new TestFile
                {
                    Filename = model.Filename,
                    Variables = variables
                };

                bool updateTestFile = _testsClient.UpdateTestVariables(testFile);
                if (updateTestFile)
                {
                    return RedirectToAction("View", "Test", new { filename = model.Filename });
                }
            }

            if (model.Variables != null)
            {
                SelectListItem[] availableEnvironments = GetEnvironmentsDropDown();
                foreach (var variable in model.Variables)
                {
                    variable.AvailableEnvironments = availableEnvironments;
                }
            }

            return View("Update", model);
        }

        [HttpGet]
        [EditableTestsRequired]
        public ActionResult AddVariableItem()
        {
            var model = new VariableViewModel
            {
                AvailableEnvironments = GetEnvironmentsDropDown()
            };

            return PartialView("EditorTemplates/VariableViewModel", model);
        }

        [HttpPost]
        [EditableTestsRequired]
        public ActionResult Copy(string sourceTestFile, string targetTestFile)
        {
            _testsClient.CopyTestFile(sourceTestFile, targetTestFile);

            return RedirectToAction("Index", "Home");
        }

        private SelectListItem[] GetEnvironmentsDropDown()
        {
            List<Environment> environments = _environmentsService.List().ToList();

            List<SelectListItem> items = environments
                .OrderBy(x => x.Order)
                .Select(x => new SelectListItem { Value = x.Name, Text = x.Name })
                .ToList();

            items.Insert(0, new SelectListItem { Value = DEFAULT_ENV_TEXT, Text = string.Empty });
            return items.ToArray();
        }
    }
}
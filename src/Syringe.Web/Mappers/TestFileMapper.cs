using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;
using Syringe.Web.Models;
using HeaderItem = Syringe.Core.Tests.HeaderItem;

namespace Syringe.Web.Mappers
{
    public class TestFileMapper : ITestFileMapper
    {
        private readonly IConfigurationService _configurationService;
        private readonly IEnvironmentsService _environmentsService;

        public TestFileMapper(IConfigurationService configurationService, IEnvironmentsService environmentsService)
        {
            _configurationService = configurationService;
            _environmentsService = environmentsService;
        }

        public TestViewModel BuildTestViewModel(TestFile testFile, int position, int pageNo = 1)
        {
            if (testFile == null)
            {
                throw new ArgumentNullException(nameof(testFile));
            }

            Test test = testFile.Tests.Skip(position).First();

            MethodType methodType;
            if (!Enum.TryParse(test.Method, true, out methodType))
            {
                methodType = MethodType.GET;
            }

            var model = new TestViewModel
            {
                Position = position,
                Filename = testFile.Filename,
                Headers = test.Headers.Select(x => new Models.HeaderItem { Key = x.Key, Value = x.Value }).ToList(),
                CapturedVariables = test.CapturedVariables.Select(x => new CapturedVariableItem { Name = x.Name, Regex = x.Regex, PostProcessorType = x.PostProcessorType }).ToList(),
                PostBody = test.PostBody,
                Method = methodType,
                ExpectedHttpStatusCode = test.ExpectedHttpStatusCode,
                Description = test.Description,
                Url = test.Url,
                Assertions = test.Assertions.Select(x => new AssertionViewModel { Value = x.Value, Description = x.Description, AssertionType = x.AssertionType, AssertionMethod = x.AssertionMethod }).ToList(),
                AvailableVariables = BuildVariableViewModel(testFile),
                BeforeExecuteScriptFilename = test.ScriptSnippets.BeforeExecuteFilename,
                PageNumber = pageNo,
                RequiredEnvironments = test.TestConditions.RequiredEnvironments,
                Environments = _environmentsService.Get().OrderBy(x => x.Order).Select(x => x.Name).ToList()
            };

            PopulateScriptSnippets(model);

            return model;
        }

        public void PopulateScriptSnippets(TestViewModel model)
        {
            model.BeforeExecuteScriptSnippets = _configurationService.GetScriptSnippetFilenames(ScriptSnippetType.BeforeExecute);
        }

        public IEnumerable<TestViewModel> BuildTests(IEnumerable<Test> tests, int pageNumber, int noOfResults)
        {
            if (tests == null)
            {
                throw new ArgumentNullException(nameof(tests));
            }

            Test[] testsArray = tests.ToArray();
            var result = new List<TestViewModel>(testsArray.Length);
            for (int i = 0; i < testsArray.Length; i++)
            {
                Test test = testsArray.ElementAt(i);
                int position = (pageNumber - 1) * noOfResults + i;

                result.Add(new TestViewModel
                {
                    Position = position,
                    Description = test.Description,
                    Url = test.Url,
                    Assertions = test.Assertions.Select(y => new AssertionViewModel { Value = y.Value, Description = y.Description, AssertionType = y.AssertionType, AssertionMethod = y.AssertionMethod }).ToList(),
                    CapturedVariables = test.CapturedVariables.Select(y => new CapturedVariableItem { Name = y.Name, Regex = y.Regex }).ToList(),
                    RequiredEnvironments = test.TestConditions.RequiredEnvironments
                });
            }

            return result;
        }

        public Test BuildTestObject(TestViewModel testModel)
        {
            if (testModel == null)
            {
                throw new ArgumentNullException(nameof(testModel));
            }

            return new Test
            {
                Headers = testModel.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList(),
                CapturedVariables = testModel.CapturedVariables.Select(x => new CapturedVariable(x.Name, x.Regex, x.PostProcessorType)).ToList(),
                PostBody = testModel.PostBody,
                Assertions = testModel.Assertions.Select(x => new Assertion(x.Description, x.Value, x.AssertionType, x.AssertionMethod)).ToList(),
                Description = testModel.Description,
                Url = testModel.Url,
                Method = testModel.Method.ToString(),
                ExpectedHttpStatusCode = testModel.ExpectedHttpStatusCode,
                ScriptSnippets = new ScriptSnippets { BeforeExecuteFilename = testModel.BeforeExecuteScriptFilename },
                TestConditions = new TestConditions { RequiredEnvironments = testModel.RequiredEnvironments ?? new List<string>(0) }
            };
        }

        public List<VariableViewModel> BuildVariableViewModel(TestFile testFile)
        {
            if (testFile == null)
            {
                throw new ArgumentNullException(nameof(testFile));
            }

            var variables = new List<VariableViewModel>();

            IEnumerable<IVariable> systemVariables = _configurationService.GetSystemVariables();
            variables.AddRange(systemVariables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value, Environment = x.Environment?.Name }));//TODO: Add in shared
            variables.AddRange(testFile.Variables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value, Environment = x.Environment?.Name }));
            variables.AddRange(testFile.Tests.SelectMany(x => x.CapturedVariables).Select(x => new VariableViewModel { Name = x.Name, Value = x.Regex }));
            return variables;
        }
    }
}
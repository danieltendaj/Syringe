using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;
using Syringe.Web.Models;
using HeaderItem = Syringe.Core.Tests.HeaderItem;

namespace Syringe.Web.Mappers
{
    public class TestFileMapper : ITestFileMapper
    {
        private readonly IConfigurationService _configurationService;

        public TestFileMapper(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public TestViewModel BuildTestViewModel(TestFile testFile, int position)
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
                BeforeExecuteScript = test.BeforeExecuteScript,
            };

            return model;
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
                });
            }

            return result;
        }

        public Test BuildCoreModel(TestViewModel testModel)
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
                BeforeExecuteScript = testModel.BeforeExecuteScript,
            };
        }

        public List<VariableViewModel> BuildVariableViewModel(TestFile testFile)
        {
            if (testFile == null)
            {
                throw new ArgumentNullException(nameof(testFile));
            }

            var variables = new List<VariableViewModel>();

            IEnumerable<Variable> systemVariables = _configurationService.GetSystemVariables();
            variables.AddRange(systemVariables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value, Environment = x.Environment?.Name }));//TODO: Add in shared
            variables.AddRange(testFile.Variables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value, Environment = x.Environment?.Name }));
            variables.AddRange(testFile.Tests.SelectMany(x => x.CapturedVariables).Select(x => new VariableViewModel { Name = x.Name, Value = x.Regex }));
            return variables;
        }
    }
}
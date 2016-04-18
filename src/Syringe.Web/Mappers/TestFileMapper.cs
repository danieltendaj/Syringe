using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Tests;
using Syringe.Web.Models;
using HeaderItem = Syringe.Core.Tests.HeaderItem;

namespace Syringe.Web.Mappers
{
    public class TestFileMapper : ITestFileMapper
    {
        public TestViewModel BuildViewModel(Test test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            MethodType methodType;
            if (!Enum.TryParse(test.Method, true, out methodType))
            {
                methodType = MethodType.GET;
            }

            var model = new TestViewModel
            {
                Position = test.Position,
                Headers = test.Headers.Select(x => new Models.HeaderItem { Key = x.Key, Value = x.Value }).ToList(),
                CapturedVariables = test.CapturedVariables.Select(x => new CapturedVariableItem { Name = x.Name, Regex = x.Regex }).ToList(),
                PostBody = test.PostBody,
                Method = methodType,
                ExpectedHttpStatusCode = test.ExpectedHttpStatusCode,
                Description = test.Description,
                Url = test.Url,
                Assertions = test.Assertions.Select(x => new AssertionViewModel { Value = x.Value, Description = x.Description, AssertionType = x.AssertionType, AssertionMethod = x.AssertionMethod}).ToList(),
                Filename = test.Filename,
                AvailableVariables = test.AvailableVariables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value }).ToList()
            };

            return model;
        }

        public IEnumerable<TestViewModel> BuildTests(IEnumerable<Test> tests)
        {
            if (tests == null)
            {
                throw new ArgumentNullException(nameof(tests));
            }

            return tests.Select(x => new TestViewModel()
            {
                Position = x.Position,
                Description = x.Description,
                Url = x.Url,
				Assertions = x.Assertions.Select(y => new AssertionViewModel { Value = y.Value, Description = y.Description, AssertionType = y.AssertionType, AssertionMethod = y.AssertionMethod }).ToList(),
				CapturedVariables = x.CapturedVariables.Select(y => new CapturedVariableItem { Name = y.Name, Regex = y.Regex }).ToList(),
			});
        }

        public Test BuildCoreModel(TestViewModel testModel)
        {
            if (testModel == null)
            {
                throw new ArgumentNullException(nameof(testModel));
            }

            return new Test
            {
                Position = testModel.Position,
                Headers = testModel.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList(),
                Filename = testModel.Filename,
                CapturedVariables = testModel.CapturedVariables.Select(x => new CapturedVariable(x.Name, x.Regex)).ToList(),
                PostBody = testModel.PostBody,
                Assertions = testModel.Assertions.Select(x => new Assertion(x.Description, x.Value, x.AssertionType, x.AssertionMethod)).ToList(),
                Description = testModel.Description,
                Url = testModel.Url,
                Method = testModel.Method.ToString(),
                ExpectedHttpStatusCode = testModel.ExpectedHttpStatusCode,
            };
        }

        public List<VariableViewModel> BuildVariableViewModel(TestFile testFile)
        {
            if (testFile == null)
            {
                throw new ArgumentNullException(nameof(testFile));
            }

            var variables = new List<VariableViewModel>();
            variables.AddRange(testFile.Variables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value }).ToList());
            variables.AddRange(testFile.Tests.SelectMany(x => x.CapturedVariables).Select(x => new VariableViewModel { Name = x.Name, Value = x.Regex }).ToList());
            return variables;
        }
    }
}
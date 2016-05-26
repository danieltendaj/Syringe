using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Variables;
using Syringe.Web.Mappers;
using Syringe.Web.Models;
using HeaderItem = Syringe.Web.Models.HeaderItem;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class TestFileMapperTests
    {
        private TestViewModel _testViewModel
        {
            get
            {
                return new TestViewModel
                {
                    Headers = new List<HeaderItem> { new HeaderItem { Key = "Key", Value = "Value" } },
                    Position = 1,
                    Filename = "Test.xml",
                    CapturedVariables = new List<CapturedVariableItem>() { new CapturedVariableItem { Name = "Description", Regex = "Regex" } },
                    PostBody = "Post Body",
                    Assertions = new List<AssertionViewModel>()
                    {
                        new AssertionViewModel { Description = "Description1", Value = "Value1", AssertionType = AssertionType.Negative, AssertionMethod = AssertionMethod.Regex },
                        new AssertionViewModel { Description = "Description2", Value = "Value2", AssertionType = AssertionType.Positive, AssertionMethod = AssertionMethod.CSQuery }
                    },
                    Description = "short d3escription",
                    Url = "url",
                    Method = MethodType.POST,
                    ExpectedHttpStatusCode = HttpStatusCode.Accepted,
                    BeforeExecuteScript = "ISomething something = new Something();"
                };
            }
        }

        [Test]
        public void Build_should_set_correct_properties_when_model_is_populated()
        {
            // given
            var testFileMapper = new TestFileMapper();

            // when
            Test test = testFileMapper.BuildCoreModel(_testViewModel);

            // then
            Assert.AreEqual(_testViewModel.Headers.Count, test.Headers.Count);
            Assert.AreEqual(_testViewModel.CapturedVariables.Count, test.CapturedVariables.Count);
            Assert.AreEqual(_testViewModel.PostBody, test.PostBody);
            Assert.AreEqual(2, test.Assertions.Count);
            Assert.AreEqual(_testViewModel.Description, test.Description);
            Assert.AreEqual(_testViewModel.Url, test.Url);
            Assert.AreEqual(_testViewModel.Method.ToString(), test.Method);
            Assert.AreEqual(_testViewModel.ExpectedHttpStatusCode, test.ExpectedHttpStatusCode);
            Assert.AreEqual(_testViewModel.BeforeExecuteScript, test.BeforeExecuteScript);
        }

        [Test]
        public void Build_should_set_assertion_properties()
        {
            // given
            var testFileMapper = new TestFileMapper();

            // when
            Test test = testFileMapper.BuildCoreModel(_testViewModel);

            // then
            Assertion firstAssertion = test.Assertions.First();
            Assert.That(firstAssertion.Description, Is.EqualTo("Description1"));
            Assert.That(firstAssertion.Value, Is.EqualTo("Value1"));
            Assert.That(firstAssertion.AssertionType, Is.EqualTo(AssertionType.Negative));
            Assert.That(firstAssertion.AssertionMethod, Is.EqualTo(AssertionMethod.Regex));

            Assertion lastAssertion = test.Assertions.Last();
            Assert.That(lastAssertion.Description, Is.EqualTo("Description2"));
            Assert.That(lastAssertion.Value, Is.EqualTo("Value2"));
            Assert.That(lastAssertion.AssertionType, Is.EqualTo(AssertionType.Positive));
            Assert.That(lastAssertion.AssertionMethod, Is.EqualTo(AssertionMethod.CSQuery));
        }

        [Test]
        public void BuildCoreModel_should_throw_argumentnullexception_when_test_is_null()
        {
            var testFileMapper = new TestFileMapper();

            Assert.Throws<ArgumentNullException>(() => testFileMapper.BuildCoreModel(null));
        }

        [Test]
        public void BuildViewModel_should_throw_argumentnullexception_when_test_is_null()
        {
            var testFileMapper = new TestFileMapper();

            Assert.Throws<ArgumentNullException>(() => testFileMapper.BuildTestViewModel(null, 0));
        }

        [Test]
        public void BuildTests_should_throw_argumentnullexception_when_test_is_null()
        {
            var testFileMapper = new TestFileMapper();

            Assert.Throws<ArgumentNullException>(() => testFileMapper.BuildTests(null, 0, 0));
        }

        [Test]
        public void BuildTests_should_return_correct_model_values_from_testfile()
        {
            // given
            var testFileMapper = new TestFileMapper();
            const int pageNumber = 2;
            const int numberOfResults = 10;
            var testFile = new TestFile
            {
                Tests = new List<Test>
                {
                    new Test
                    {
                        Description = "Description 1",
                        Url = "http://www.google.com",
                        Assertions = new List<Assertion>() { new Assertion(), new Assertion()},
                        CapturedVariables = new List<CapturedVariable>() { new CapturedVariable(), new CapturedVariable() }
                    },
                    new Test
                    {
                        Description = "Description 2",
                        Url = "http://www.arsenal.com",
                        Assertions = new List<Assertion>() { new Assertion(), new Assertion(), new Assertion()},
                        CapturedVariables = new List<CapturedVariable>() { new CapturedVariable(), new CapturedVariable(), new CapturedVariable() }
                    },
                }
            };

            // when
            IEnumerable<TestViewModel> viewModels = testFileMapper.BuildTests(testFile.Tests, pageNumber, numberOfResults);

            // then
            Assert.NotNull(viewModels);
            Assert.AreEqual(2, viewModels.Count());

            var firstCase = viewModels.First();
            Assert.AreEqual(10, firstCase.Position);
            Assert.AreEqual("Description 1", firstCase.Description);
            Assert.AreEqual("http://www.google.com", firstCase.Url);
            Assert.AreEqual("http://www.google.com", firstCase.Url);
            Assert.That(firstCase.Assertions.Count, Is.EqualTo(2));
            Assert.That(firstCase.CapturedVariables.Count, Is.EqualTo(2));

            var lastCase = viewModels.Last();
            Assert.AreEqual(11, lastCase.Position);
            Assert.AreEqual("Description 2", lastCase.Description);
            Assert.That(lastCase.Assertions.Count, Is.EqualTo(3));
            Assert.That(lastCase.CapturedVariables.Count, Is.EqualTo(3));
        }
        
        [Test]
        public void BuildTestViewModel_should_return_correct_model_values_from_test()
        {
            // given
            const int testPosition = 1;
            var fileMapper = new TestFileMapper();
            var expectedTest = new Test
            {
                Description = "Short Description",
                Url = "http://www.google.com",
                Method = MethodType.GET.ToString(),
                PostBody = "PostBody",
                ExpectedHttpStatusCode = HttpStatusCode.Accepted,
                Headers = new List<Syringe.Core.Tests.HeaderItem> { new Syringe.Core.Tests.HeaderItem() },
                CapturedVariables = new List<CapturedVariable> { new CapturedVariable { Name = "CV-2" } },
                Assertions = new List<Assertion> { new Assertion("Desc", "Val", AssertionType.Negative, AssertionMethod.CSQuery) },
                BeforeExecuteScript = "// this is some script"
            };

            var testFile = new TestFile
            {
                Filename = "some file name...YURP",
                Tests = new List<Test>
                {
                    new Test
                    {
                        CapturedVariables = new List<CapturedVariable>
                        {
                            new CapturedVariable("CV-1", "CV-1-Value")
                        }
                    },
                    expectedTest,
                    new Test()
                },
                Variables = new List<Variable>
                {
                    new Variable("V-1", "V-1-Value", null)
                }
            };

            // when
            TestViewModel result = fileMapper.BuildTestViewModel(testFile, testPosition);

            // then
            Assert.NotNull(result);
            Assert.That(result.Position, Is.EqualTo(testPosition));
            Assert.That(result.Description, Is.EqualTo(expectedTest.Description));
            Assert.That(result.Url, Is.EqualTo(expectedTest.Url));
            Assert.That(result.PostBody, Is.EqualTo(expectedTest.PostBody));
            Assert.That(result.Method, Is.EqualTo(MethodType.GET));
            Assert.That(result.ExpectedHttpStatusCode, Is.EqualTo(expectedTest.ExpectedHttpStatusCode));
            Assert.That(result.Filename, Is.EqualTo(testFile.Filename));
            Assert.That(result.BeforeExecuteScript, Is.EqualTo(expectedTest.BeforeExecuteScript));

            Assert.That(result.CapturedVariables.Count, Is.EqualTo(1));
            Assert.That(result.Assertions.Count, Is.EqualTo(1));
            Assert.That(result.Headers.Count, Is.EqualTo(1));

            AssertionViewModel assertionViewModel = result.Assertions.FirstOrDefault();
            Assert.That(assertionViewModel.Description, Is.EqualTo("Desc"));
            Assert.That(assertionViewModel.Value, Is.EqualTo("Val"));
            Assert.That(assertionViewModel.AssertionType, Is.EqualTo(AssertionType.Negative));
            Assert.That(assertionViewModel.AssertionMethod, Is.EqualTo(AssertionMethod.CSQuery));

            Assert.That(result.AvailableVariables.Count, Is.EqualTo(3));

            VariableViewModel capturedVar = result.AvailableVariables.Find(x => x.Name == "CV-1");
            Assert.That(capturedVar, Is.Not.Null);
            Assert.That(capturedVar.Value, Is.EqualTo("CV-1-Value"));

            capturedVar = result.AvailableVariables.Find(x => x.Name == "CV-2");
            Assert.That(capturedVar, Is.Not.Null);

            VariableViewModel testVariable = result.AvailableVariables.Find(x => x.Name == "V-1");
            Assert.That(testVariable, Is.Not.Null);
            Assert.That(testVariable.Value, Is.EqualTo("V-1-Value"));
        }

        [Test]
        public void BuildVariableViewModel_should_throw_exception_when_test_is_null()
        {
            // given
            var fileMapper = new TestFileMapper();

            // when + then
            Assert.Throws<ArgumentNullException>(() => fileMapper.BuildVariableViewModel(null));
        }

        [Test]
        public void BuildVariableViewModel_should_return_base_variables_if_they_exist()
        {
            // given
            var fileMapper = new TestFileMapper();
            var testFile = new TestFile { Variables = new List<Variable> { new Variable { Name = "test", Value = "value" } } };

            // when + then
            var buildVariableViewModel = fileMapper.BuildVariableViewModel(testFile);
            Assert.AreEqual(1, buildVariableViewModel.Count);
            Assert.AreEqual("test", buildVariableViewModel[0].Name);
            Assert.AreEqual("value", buildVariableViewModel[0].Value);
        }

        [Test]
        public void BuildVariableViewModel_should_return_captured_variables_if_they_exist()
        {
            // given
            var fileMapper = new TestFileMapper();
            var testFile = new TestFile
            {
                Tests = new List<Test>
                {
                    new Test
                    {
                        CapturedVariables = new List<CapturedVariable>
                        {
                            new CapturedVariable("name", "regex")
                        }
                    },
                    new Test()
                }
            };

            // when + then
            List<VariableViewModel> buildVariableViewModel = fileMapper.BuildVariableViewModel(testFile);
            Assert.That(buildVariableViewModel.Count, Is.EqualTo(1));
            Assert.That(buildVariableViewModel[0].Name, Is.EqualTo("name"));
            Assert.That(buildVariableViewModel[0].Value, Is.EqualTo("regex"));
        }
    }
}

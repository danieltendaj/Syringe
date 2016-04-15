using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;
using HeaderItem = Syringe.Web.Models.HeaderItem;

namespace Syringe.Tests.Unit.ModelBuilders
{
    [TestFixture]
    public class TestFileMapperTests
    {
        [Test]
        public void Build_should_set_correct_properties_when_model_is_populated()
        {
            var testFileMapper = new TestFileMapper();
            var build = testFileMapper.BuildCoreModel(testViewModel);

            Assert.AreEqual(testViewModel.ErrorMessage, build.ErrorMessage);
            Assert.AreEqual(testViewModel.Headers.Count, build.Headers.Count);
            Assert.AreEqual(testViewModel.Position, build.Position);
            Assert.AreEqual(testViewModel.LongDescription, build.LongDescription);
            Assert.AreEqual(testViewModel.Filename, build.Filename);
            Assert.AreEqual(testViewModel.CapturedVariables.Count, build.CapturedVariables.Count);
            Assert.AreEqual(testViewModel.PostBody, build.PostBody);
            Assert.AreEqual(2, build.Assertions.Count);
            Assert.AreEqual(testViewModel.ShortDescription, build.ShortDescription);
            Assert.AreEqual(testViewModel.Url, build.Url);
            Assert.AreEqual(testViewModel.Method.ToString(), build.Method);
            Assert.AreEqual(testViewModel.VerifyResponseCode, build.VerifyResponseCode);
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

            Assert.Throws<ArgumentNullException>(() => testFileMapper.BuildViewModel(null));
        }

        [Test]
        public void BuildTests_should_throw_argumentnullexception_when_test_is_null()
        {
            var testFileMapper = new TestFileMapper();

            Assert.Throws<ArgumentNullException>(() => testFileMapper.BuildTests(null));
        }

        [Test]
        public void BuildTests_should_return_correct_model_values_from_testfile()
        {
            // given
            var testFileMapper = new TestFileMapper();
            var testFileId1 = 1;
            var testFileId2 = 2;
            var testFile = new TestFile
            {
                Tests = new List<Test>
                {
                    new Test {Position = testFileId1, ShortDescription = "Short Description 1", Url = "http://www.google.com"},
                    new Test {Position = testFileId2, ShortDescription = "Short Description 2", Url = "http://www.arsenal.com"},
                }
            };

            // when
            IEnumerable<TestViewModel> viewModels = testFileMapper.BuildTests(testFile.Tests);

            // then
            Assert.NotNull(viewModels);
            Assert.AreEqual(2, viewModels.Count());

            var firstCase = viewModels.First();
            Assert.AreEqual(testFileId1, firstCase.Position);
            Assert.AreEqual("Short Description 1", firstCase.ShortDescription);
            Assert.AreEqual("http://www.google.com", firstCase.Url);

            var lastCase = viewModels.Last();
            Assert.AreEqual(testFileId2, lastCase.Position);
            Assert.AreEqual("Short Description 2", lastCase.ShortDescription);
            Assert.AreEqual("http://www.arsenal.com", lastCase.Url);
        }

        [Test]
        public void BuildViewModel_should_return_correct_model_values_from_test()
        {
            // given
            var fileMapper = new TestFileMapper();

            var test = new Test
            {
                Position = 1,
                ShortDescription = "Short Description",
                Url = "http://www.google.com",
                ErrorMessage = "Error",
                LongDescription = "Long Description",
                Method = MethodType.GET.ToString(),
                PostBody = "PostBody",
                //PostType = MethodType.GET.ToString(),
                VerifyResponseCode = HttpStatusCode.Accepted,
                Headers = new List<Core.Tests.HeaderItem> { new Core.Tests.HeaderItem() },
                CapturedVariables = new List<CapturedVariable> { new CapturedVariable() },
                Assertions = new List<Assertion> { new Assertion() },
                Filename = "test.xml",
            };

            // when
            TestViewModel testViewModel = fileMapper.BuildViewModel(test);

            // then
            Assert.NotNull(testViewModel);
            Assert.AreEqual(test.Position, testViewModel.Position);
            Assert.AreEqual(test.ShortDescription, testViewModel.ShortDescription);
            Assert.AreEqual(test.Url, testViewModel.Url);
            Assert.AreEqual(test.ErrorMessage, testViewModel.ErrorMessage);
            Assert.AreEqual(test.LongDescription, testViewModel.LongDescription);
            Assert.AreEqual(test.PostBody, testViewModel.PostBody);
            Assert.AreEqual(MethodType.GET, testViewModel.Method);
            Assert.AreEqual(test.VerifyResponseCode, testViewModel.VerifyResponseCode);
            Assert.AreEqual(test.Filename, testViewModel.Filename);

            Assert.AreEqual(1, testViewModel.CapturedVariables.Count);
            Assert.AreEqual(1, testViewModel.Assertions.Count);
            Assert.AreEqual(1, test.Headers.Count);
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
            var testFile = new TestFile { Tests = new List<Test> { new Test { CapturedVariables = new List<CapturedVariable> { new CapturedVariable("name", "regex") } }, new Test() } };

            // when + then
            var buildVariableViewModel = fileMapper.BuildVariableViewModel(testFile);
            Assert.AreEqual(1, buildVariableViewModel.Count);
            Assert.AreEqual("name", buildVariableViewModel[0].Name);
            Assert.AreEqual("regex", buildVariableViewModel[0].Value);
        }

        private TestViewModel testViewModel
        {
            get
            {
                return new TestViewModel
                {
                    ErrorMessage = "error",
                    Headers = new List<HeaderItem> { new HeaderItem { Key = "Key", Value = "Value" } },
                    Position = 1,
                    LongDescription = "long description",
                    Filename = "Test.xml",
                    CapturedVariables = new List<CapturedVariableItem>() { new CapturedVariableItem { Description = "Description", Regex = "Regex" } },
                    PostBody = "Post Body",
                    Assertions = new List<AssertionViewModel>() { new AssertionViewModel { Description = "Description", Regex = "Regex", AssertionType = AssertionType.Negative },
                               new AssertionViewModel { Description = "Description", Regex = "Regex", AssertionType = AssertionType.Positive } },
                    ShortDescription = "short d3escription",
                    Url = "url",
                    Method = MethodType.POST,
                    VerifyResponseCode = HttpStatusCode.Accepted,
                };
            }
        }
    }
}

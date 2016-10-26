using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Runner;
using Syringe.Core.Runner.Logging;
using Syringe.Core.Runner.Messaging;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Core.Tests.Variables;
using Syringe.Tests.StubsMocks;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Tests.Unit.Core.Runner
{
    public class TestFileRunnerTests
    {
        private HttpClientMock _httpClientMock;
        private HttpResponse _httpResponse;
        private Mock<ICapturedVariableProviderFactory> _capturedVariableProviderFactory;
        private CapturedVariableProviderStub _capturedVariableProvider;

        [SetUp]
        public void Setup()
        {
            _capturedVariableProviderFactory = new Mock<ICapturedVariableProviderFactory>();
            _capturedVariableProvider = new CapturedVariableProviderStub();

            _capturedVariableProviderFactory
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(_capturedVariableProvider);
        }

        private ITestFileResultRepositoryFactory GetRepositoryFactory()
        {
            return new TestFileResultRepositoryFactoryMock();
        }

        private ITestFileRunnerLoggerFactory GetTestFileRunnerLoggerFactory()
        {
            return new TestFileRunnerLogFactoryMock();
        }

        [Test]
        public async Task Run_should_set_MinResponseTime_and_MaxResponseTime_from_http_response_times()
        {
            // given
            var response = new HttpResponse();
            response.ResponseTime = TimeSpan.FromSeconds(5);

            HttpClientMock httpClient = new HttpClientMock(response);
            httpClient.ResponseTimes = new List<TimeSpan>()
            {
				// Deliberately mixed up order
				TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(88),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(10)
            };
            httpClient.Response = response;

            var runner = new TestFileRunner(httpClient, GetRepositoryFactory(), new JsonConfiguration(), _capturedVariableProviderFactory.Object, GetTestFileRunnerLoggerFactory());

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
                new Test() { Url = "foo2" },
                new Test() { Url = "foo3" },
                new Test() { Url = "foo4" },
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(session.MinResponseTime, Is.EqualTo(TimeSpan.FromSeconds(3)));
            Assert.That(session.MaxResponseTime, Is.EqualTo(TimeSpan.FromSeconds(88)));
        }

        [Test]
        public async Task Run_should_populate_StartTime_and_EndTime_and_TotalRunTime()
        {
            // given
            var beforeStart = DateTime.UtcNow;

            var response = new HttpResponse();
            response.ResponseTime = TimeSpan.FromSeconds(5);

            HttpClientMock httpClient = new HttpClientMock(response);
            var runner = new TestFileRunner(httpClient, GetRepositoryFactory(), new JsonConfiguration(), _capturedVariableProviderFactory.Object, GetTestFileRunnerLoggerFactory());

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(session.StartTime, Is.GreaterThanOrEqualTo(beforeStart));
            Assert.That(session.EndTime, Is.GreaterThanOrEqualTo(session.StartTime));
            Assert.That(session.TotalRunTime, Is.EqualTo(session.EndTime - session.StartTime));
        }

        [Test]
        public async Task Run_should_set_capturedvariables()
        {
            // given
            const string environment = "big-daddy-doo-dah";
            const string httpContent = "THIS IS SOME CONTENT - content coming to you 24/7, 365, every and all init yeah.";

            TestFileRunner runner = CreateRunner();
            _httpClientMock.Response.Content = httpContent;
            _httpClientMock.Response.StatusCode = HttpStatusCode.OK;

            var testFile = CreateTestFile(new[]
            {
                new Test()
                {
                    Url = "test1",
                    ExpectedHttpStatusCode = HttpStatusCode.OK,
                    CapturedVariables = new List<CapturedVariable>()
                    {
                        new CapturedVariable("var1", "some content (.*?every)")
                    },
                    Assertions = new List<Assertion>()
                    {
                        new Assertion("positive-1", httpContent.Substring(0, 10), AssertionType.Positive, AssertionMethod.Regex)
                    },
                }
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, environment, "bob");

            // then
            var addedVariable = _capturedVariableProvider.Variables.FirstOrDefault(x => x.Name == "var1");
            Assert.That(addedVariable, Is.Not.Null);
            Assert.That(addedVariable.Value, Is.EqualTo("- content coming to you 24/7, 365, every"));

            var testResult = session.TestResults.Single();
            Assert.That(testResult.AssertionResults[0].Success, Is.True);
        }

        [Test]
        public async Task Run_should_ignore_null_scriptevaluator_output()
        {
            // given
            const string environment = "knights-of-the-white-table";
            const string httpContent = "im lowercase";

            TestFileRunner runner = CreateRunner();
            _httpClientMock.Response.Content = httpContent;
            _httpClientMock.Response.StatusCode = HttpStatusCode.OK;

            var testFile = CreateTestFile(new[]
            {
                new Test()
                {
                    Url = "test1",
                    ExpectedHttpStatusCode = HttpStatusCode.OK,
                }
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, environment, "bob");

            // then
            Assert.That(testFile, Is.Not.Null);
            Assert.That(testFile.Tests.FirstOrDefault(), Is.Not.Null);
            Assert.That(session.TestResults.First().HttpResponse, Is.Not.Null);
        }

        [Test]
        public async Task Run_should_set_capturedvariables_across_tests()
        {
            // given
            const string environment = "big-daddy-doo-dah";

            TestFileRunner runner = CreateRunner();
            _httpClientMock.Responses = new List<HttpResponse>()
            {
                new HttpResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = "1st content SECRET_KEY"
                },
                new HttpResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = "2nd content - SECRET_KEY in here to match"
                },
                new HttpResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = "3rd content - SECRET_KEY in here to match"
                }
            };

            var testFile = CreateTestFile(new[]
            {
                new Test()
                {
                    Url = "test1",
                    ExpectedHttpStatusCode = HttpStatusCode.OK,
                    CapturedVariables = new List<CapturedVariable>()
                    {
                        new CapturedVariable("var1", @"(SECRET_KEY)")
                    },
                },
                new Test()
                {
                    Url = "test2",
                    ExpectedHttpStatusCode = HttpStatusCode.OK,
                    CapturedVariables = new List<CapturedVariable>()
                    {
                        new CapturedVariable("var2", @"(SECRET_KEY)")
                    },
                    Assertions = new List<Assertion>()
                    {
						// Test the capturedvariable variable from the 1st test
						new Assertion("positive-for-test-2", "{var1}", AssertionType.Positive, AssertionMethod.Regex)
                    },
                },
                new Test()
                {
                    Url = "test3",
                    ExpectedHttpStatusCode = HttpStatusCode.OK,
                    Assertions = new List<Assertion>()
                    {
						// Test the capturedvariable variable from the 1st test
						new Assertion("positive-for-test-3", "{var2}", AssertionType.Positive, AssertionMethod.Regex)
                    },
                }
            });

            // when
            await runner.RunAsync(testFile, environment, "bob");

            // then
            var addedVariable = _capturedVariableProvider.Variables.FirstOrDefault(x => x.Name == "var1");
            Assert.That(addedVariable, Is.Not.Null);
            Assert.That(addedVariable.Value, Is.EqualTo("SECRET_KEY"));

            addedVariable = _capturedVariableProvider.Variables.FirstOrDefault(x => x.Name == "var2");
            Assert.That(addedVariable, Is.Not.Null);
            Assert.That(addedVariable.Value, Is.EqualTo("SECRET_KEY"));
        }

        [Test]
        public async Task Run_should_set_testresult_success_and_response_when_httpcode_passes()
        {
            // given
            TestFileRunner runner = CreateRunner();
            _httpClientMock.Response.StatusCode = HttpStatusCode.OK;

            var testFile = CreateTestFile(new[]
            {
                new Test()
                {
                    Url = "foo1",
                    ExpectedHttpStatusCode = HttpStatusCode.OK
                },
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(session.TestResults.Single().Success, Is.True);
            Assert.That(session.TestResults.Single().ResultState, Is.EqualTo(TestResultState.Success));
            Assert.That(session.TestResults.Single().HttpResponse, Is.EqualTo(_httpClientMock.Response));
        }

        [Test]
        public async Task Run_should_set_testresult_success_and_response_when_httpcode_fails()
        {
            // given
            TestFileRunner runner = CreateRunner();
            _httpClientMock.Response.StatusCode = HttpStatusCode.OK;

            var testFile = CreateTestFile(new[]
            {
                new Test()
                {
                    Url = "foo1",
                    ExpectedHttpStatusCode = HttpStatusCode.Ambiguous
                },
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(session.TestResults.Single().ResultState, Is.EqualTo(TestResultState.Failed));
            Assert.That(session.TestResults.Single().Success, Is.False);
            Assert.That(session.TestResults.Single().HttpResponse, Is.EqualTo(_httpClientMock.Response));
        }

        [Test]
        public async Task Run_should_save_testresults_to_repository()
        {
            // given
            var repositoryFactory = new TestFileResultRepositoryFactoryMock();

            TestFileRunner runner = CreateRunner();
            runner.RepositoryFactory = repositoryFactory;

            var repository = (TestFileResultRepositoryMock)repositoryFactory.GetRepository();

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" }
            });

            // when
            await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(repository.SavedTestFileResult, Is.Not.Null);
            Assert.That(repository.SavedTestFileResult.TestResults.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Run_should_verify_positive_and_negative_items_when_httpcode_passes()
        {
            // given
            TestFileRunner runner = CreateRunner();
            _httpClientMock.Response.StatusCode = HttpStatusCode.OK;
            _httpClientMock.Response.Content = "some content";

            var testFile = CreateTestFile(new[]
            {
                new Test()
                {
                    Url = "foo1",
                    ExpectedHttpStatusCode = HttpStatusCode.OK,
                    Assertions = new List<Assertion>()
                    {
                        new Assertion("positive-1", "some content", AssertionType.Positive, AssertionMethod.Regex),
                        new Assertion("negative-1", "no text like this", AssertionType.Negative, AssertionMethod.Regex)
                    }
                },
            });

            // when
            TestFileResult session = await runner.RunAsync(testFile, "development", "bob");

            // then
            var result = session.TestResults.Single();
            Assert.That(result.Success, Is.True);
            Assert.That(result.ResultState, Is.EqualTo(TestResultState.Success));
            Assert.That(result.AssertionResults.Where(x => x.AssertionType == AssertionType.Positive).Count, Is.EqualTo(1));
            Assert.That(result.AssertionResults[0].Success, Is.True);

            Assert.That(result.AssertionResults.Where(x => x.AssertionType == AssertionType.Negative).Count, Is.EqualTo(1));
            Assert.That(result.AssertionResults[0].Success, Is.True);
        }

        [Test]
        public async Task Run_should_notify_observers_of_existing_results()
        {
            // given
            var observedResults = new List<TestResultMessage>();

            TestFileRunner runner = CreateRunner();

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
                new Test() { Url = "foo2" },
                new Test() { Url = "foo3" }
            });

            await runner.RunAsync(testFile, "development", "bob");

            // when
            runner.Subscribe(r => { observedResults.Add(r as TestResultMessage); });

            // then
            Assert.That(observedResults.Select(r => r.TestResult.ActualUrl), Is.EquivalentTo(new[] { "foo1", "foo2", "foo3" }), "Should have observed all of the results.");
        }

        [Test]
        public async Task Run_should_notify_observers_of_new_results()
        {
            // given
            TestFileRunner runner = CreateRunner();

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
                new Test() { Url = "foo2" },
                new Test() { Url = "foo3" }
            });

            var observedResults = new List<TestResultMessage>();

            // when
            runner.Subscribe(r =>
            {
                var item = r as TestResultMessage;
                if (item != null) observedResults.Add(item);
            });

            await runner.RunAsync(testFile, "development", "bob");

            // then
            string[] result = observedResults.Select(r => r.TestResult.ActualUrl).ToArray();
            Assert.That(result, Is.EquivalentTo(new[] { "foo1", "foo2", "foo3" }), "Should have observed all of the results.");
        }

        [Test]
        public async Task Run_should_notify_observers_of_completion()
        {
            // given
            TestFileRunner runner = CreateRunner();

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
                new Test() { Url = "foo2" },
                new Test() { Url = "foo3" }
            });

            var observedResults = new List<TestFileGuidMessage>();

            // when
            runner.Subscribe(r =>
            {
                var item = r as TestFileGuidMessage;
                if (item != null) observedResults.Add(item);
            });

            await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(observedResults.Count, Is.EqualTo(1), "Should have observed completion message.");
        }

        [Test]
        public async Task Run_should_not_notify_disposed_observers_of_new_results()
        {
            // given
            var httpClientMock = new Mock<IHttpClient>();
            IDisposable subscription = null;

            httpClientMock.Setup(c => c.CreateRestRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<HeaderItem>>()));

            // Dispose of the subscription before processing the third request.
            httpClientMock
                .Setup(c => c.CreateRestRequest(It.IsAny<string>(), "http://foo3", It.IsAny<string>(), It.IsAny<IEnumerable<HeaderItem>>()))
                .Callback(() => { subscription?.Dispose(); });

            httpClientMock
                .Setup(c => c.ExecuteRequestAsync(It.IsAny<IRestRequest>(), It.IsAny<HttpLogWriter>()))
                .Returns(Task.FromResult(new HttpResponse()));

            TestFileRunner runner = new TestFileRunner(httpClientMock.Object, GetRepositoryFactory(), new JsonConfiguration(), _capturedVariableProviderFactory.Object, GetTestFileRunnerLoggerFactory());

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "http://foo1" },
                new Test() { Url = "http://foo2" },
                new Test() { Url = "http://foo3" }
            });

            var observedResults = new List<TestResultMessage>();

            // when
            subscription = runner.Subscribe(r => { observedResults.Add(r as TestResultMessage); });

            await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(observedResults.Select(r => r.TestResult.ActualUrl), Is.EquivalentTo(new[] { "http://foo1", "http://foo2" }), "Should not have included the result after having been disposed.");
        }

        [Test]
        public async Task Run_should_notify_subscribers_of_completion_when_test_file_ends()
        {
            // given
            TestFileRunner runner = CreateRunner();

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
                new Test() { Url = "foo2" },
                new Test() { Url = "foo3" }
            });

            var completed = false;

            runner.Subscribe(r => { }, onCompleted: () => completed = true);

            Assume.That(completed, Is.False);

            // when
            await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(completed, Is.True, "Should have notified of completion.");
        }

        [Test]
        public async Task Run_should_notify_subscribers_of_result_on_error()
        {
            // given
            var httpClientMock = new Mock<IHttpClient>();

            // Throw an error.
            httpClientMock
                .Setup(c => c.ExecuteRequestAsync(It.IsAny<IRestRequest>(), new HttpLogWriter()))
                .Throws(new InvalidOperationException("Bad"));

            TestFileRunner runner = new TestFileRunner(httpClientMock.Object, GetRepositoryFactory(), new JsonConfiguration(), _capturedVariableProviderFactory.Object, GetTestFileRunnerLoggerFactory());

            var testFile = CreateTestFile(new[]
            {
                new Test() { Url = "foo1" },
                new Test() { Url = "foo2" },
                new Test() { Url = "foo3" }
            });

            TestResultMessage capturedResult = null;
            runner.Subscribe(r =>
            {
                var item = r as TestResultMessage;
                if (item != null) capturedResult = item;
            });

            // when
            await runner.RunAsync(testFile, "development", "bob");

            // then
            Assert.That(capturedResult, Is.Not.Null, "Should have notified of the result.");
            Assert.That(capturedResult.TestResult, Is.Not.Null, "Should have test result.");
            Assert.That(capturedResult.TestResult.Success, Is.False, "Should not have succeeded.");
            Assert.That(capturedResult.TestResult.ResultState, Is.EqualTo(TestResultState.Failed), "Should not have succeeded.");
        }

        private TestFileRunner CreateRunner()
        {
            _httpResponse = new HttpResponse();
            _httpClientMock = new HttpClientMock(_httpResponse);

            return new TestFileRunner(_httpClientMock, GetRepositoryFactory(), new JsonConfiguration(),
                _capturedVariableProviderFactory.Object, GetTestFileRunnerLoggerFactory());
        }

        private TestFile CreateTestFile(Test[] tests)
        {
            var list = new List<Test>();
            list.AddRange(tests);

            var testFile = new TestFile();
            testFile.Tests = list;

            return testFile;
        }
    }
}

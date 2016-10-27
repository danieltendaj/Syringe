using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Service.Models;
using Syringe.Service.Parallel;

namespace Syringe.Tests.Unit.Service.Parallel
{
    [TestFixture]
    public class TestFileResultFactoryTests
    {
        [Test]
        public void should_return_error_if_timed_out()
        {
            // given
            var timeTaken = TimeSpan.FromMinutes(1);
            var factory = new TestFileResultFactory();

            // when
            var result = factory.Create(null, true, timeTaken);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Finished, Is.False);
            Assert.That(result.TimeTaken, Is.EqualTo(timeTaken));
            Assert.That(result.ErrorMessage, Is.EqualTo("The runner timed out."));
        }

        [Test]
        public void should_return_error_if_errors_exist()
        {
            // given
            var timeTaken = TimeSpan.FromMinutes(1);
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                Errors = "WOOPEEE"
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, timeTaken);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Finished, Is.False);
            Assert.That(result.TimeTaken, Is.EqualTo(timeTaken));
            Assert.That(result.ErrorMessage, Is.EqualTo(runnerInfo.Errors));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void should_return_result_result_id_if_it_exists(bool idExists)
        {
            // given
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                TestFileResults = idExists ? new TestFileResult { Id = Guid.NewGuid() } : null
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, TimeSpan.Zero);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo(string.Empty));
            Assert.That(result.TestResults, Is.EqualTo(new LightweightResult[0]));

            if (idExists)
            {
                Assert.That(result.ResultId, Is.EqualTo(runnerInfo.TestFileResults.Id));
            }
            else
            {
                Assert.That(result.ResultId, Is.Null);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void should_detect_return_failed_count_when_failed_tests_exist(bool hasFailedTests)
        {
            // given
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                TestFileResults = new TestFileResult
                {
                    TestResults = new List<TestResult>
                    {
                        new TestResult { ResultState = hasFailedTests ? TestResultState.Failed : TestResultState.Success},
                        new TestResult { ResultState = TestResultState.Success }
                    }
                }
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, TimeSpan.Zero);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorMessage, Is.EqualTo(string.Empty));
            Assert.That(result.HasFailedTests, Is.EqualTo(hasFailedTests));
            Assert.That(result.TestResults.Count(), Is.EqualTo(2));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void should_return_completion_and_failed_status(bool completed)
        {
            // given
            var timeTaken = TimeSpan.FromMinutes(1);
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                CurrentTask = Task.Factory.StartNew(() =>
                {
                    if (!completed) { throw new Exception(); }
                })
            };

            try
            {
                runnerInfo.CurrentTask.Wait();
            }
            catch (Exception)
            {
                // ignore the error
            }

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, timeTaken);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Finished, Is.EqualTo(completed));
            Assert.That(result.TestRunFailed, Is.EqualTo(!completed));
        }

        [Test]
        public void should_return_time_taken_from_given_time()
        {
            // given
            var timeTaken = TimeSpan.FromMinutes(1);
            var runnerInfo = new TestFileRunnerTaskInfo(0);

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, timeTaken);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TimeTaken, Is.EqualTo(timeTaken));
            Assert.That(result.TestResults, Is.EqualTo(new LightweightResult[0]));
        }

        [Test]
        public void should_return_time_taken_from_test_file_result_if_not_given_in_method_call()
        {
            // given
            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                TestFileResults = new TestFileResult
                {
                    TotalRunTime = TimeSpan.FromDays(1)
                }
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, TimeSpan.Zero);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TimeTaken, Is.EqualTo(runnerInfo.TestFileResults.TotalRunTime));
        }

        [Test]
        public void should_return_test_light_results()
        {
            // given
            var testResult = new TestResult
            {
                Message = "some-message",
                ExceptionMessage = "exception-message",
                ScriptCompilationSuccess = true,
                ResponseTime = TimeSpan.FromMinutes(3),
                ResponseCodeSuccess = true,
                ActualUrl = "foo-bar",
                ResultState = TestResultState.Success,
                Test = new Test
                {
                    Url = "url-bar",
                    Description = "description-boo"
                }
            };

            var runnerInfo = new TestFileRunnerTaskInfo(0)
            {
                TestFileResults = new TestFileResult
                {
                    TestResults = new List<TestResult> { testResult }
                }
            };

            // when
            var factory = new TestFileResultFactory();
            var result = factory.Create(runnerInfo, false, TimeSpan.Zero);

            // then
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestResults.Count(), Is.EqualTo(1));

            var result1 = result.TestResults.First();
            Assert.That(result1.Success, Is.True);
            Assert.That(result1.ResultState, Is.EqualTo("Success"));
            Assert.That(result1.Message, Is.EqualTo(testResult.Message));
            Assert.That(result1.ExceptionMessage, Is.EqualTo(testResult.ExceptionMessage));
            Assert.That(result1.AssertionsSuccess, Is.EqualTo(testResult.AssertionsSuccess));
            Assert.That(result1.ScriptCompilationSuccess, Is.EqualTo(testResult.ScriptCompilationSuccess));
            Assert.That(result1.ResponseTime, Is.EqualTo(testResult.ResponseTime));
            Assert.That(result1.ResponseCodeSuccess, Is.EqualTo(testResult.ResponseCodeSuccess));
            Assert.That(result1.ActualUrl, Is.EqualTo(testResult.ActualUrl));
            Assert.That(result1.TestUrl, Is.EqualTo(testResult.Test.Url));
            Assert.That(result1.TestDescription, Is.EqualTo(testResult.Test.Description));
        }
    }
}
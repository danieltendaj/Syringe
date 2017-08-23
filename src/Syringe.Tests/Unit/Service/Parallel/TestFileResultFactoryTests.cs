using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Service.Models;
using Syringe.Service.Parallel;
using Syringe.Tests.NUnitToXUnit;
using Xunit;

namespace Syringe.Tests.Unit.Service.Parallel
{
	public class TestFileResultFactoryTests
	{
		[Fact]
		public void should_return_error_if_timed_out()
		{
			// given
			var timeTaken = TimeSpan.FromMinutes(1);
			var factory = new TestFileResultFactory();

			// when
			var result = factory.Create(null, true, timeTaken);

			// then
			Assert.NotNull(result);
			Assert.False(result.Finished);
			Assert.Equal(result.TimeTaken, timeTaken);
			Assert.Equal(result.ErrorMessage, "The runner timed out.");
		}

		[Fact]
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
			Assert.NotNull(result);
			Assert.Equal(result.TimeTaken, timeTaken);
			Assert.Equal(result.ErrorMessage, runnerInfo.Errors);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
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
			Assert.NotNull(result);
			Assert.Equal(result.ErrorMessage, string.Empty);
			Assert.Equal(result.TestResults, new LightweightResult[0]);

			if (idExists)
			{
				Assert.Equal(result.ResultId, runnerInfo.TestFileResults.Id);
			}
			else
			{
				Assert.Null(result.ResultId);
			}
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void should_detect_return_failed_count_when_failed_tests_exist(bool hasFailedTests)
		{
			// given
			var runnerInfo = new TestFileRunnerTaskInfo(0)
			{
				TestFileResults = new TestFileResult
				{
					TestResults = new List<TestResult>
					{
						new TestResult { ResponseCodeSuccess = !hasFailedTests },
						new TestResult { ResponseCodeSuccess = true } // SUCCESS ;-)
                    }
				}
			};

			// when
			var factory = new TestFileResultFactory();
			var result = factory.Create(runnerInfo, false, TimeSpan.Zero);

			// then
			Assert.NotNull(result);
			Assert.Empty(result.ErrorMessage);
			Assert.Equal(result.HasFailedTests, hasFailedTests);
			Assert.Equal(result.TestResults.Count(), 2);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
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
			Assert.NotNull(result);
			Assert.Empty(result.ErrorMessage);
			Assert.Equal(result.Finished, completed);
			Assert.Equal(result.TestRunFailed, !completed);
		}

		[Fact]
		public void should_return_time_taken_from_given_time()
		{
			// given
			var timeTaken = TimeSpan.FromMinutes(1);
			var runnerInfo = new TestFileRunnerTaskInfo(0);

			// when
			var factory = new TestFileResultFactory();
			var result = factory.Create(runnerInfo, false, timeTaken);

			// then
			Assert.NotNull(result);
			Assert.Equal(result.TimeTaken, timeTaken);
			Assert.Equal(result.TestResults, new LightweightResult[0]);
		}

		[Fact]
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
			Assert.NotNull(result);
			Assert.Equal(result.TimeTaken, runnerInfo.TestFileResults.TotalRunTime);
		}

		[Fact]
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
			Assert.NotNull(result);
			Assert.Equal(result.TestResults.Count(), 1);

			var result1 = result.TestResults.First();
			Assert.Equal(result1.Success, testResult.Success);
			Assert.Equal(result1.Message, testResult.Message);
			Assert.Equal(result1.ExceptionMessage, testResult.ExceptionMessage);
			Assert.Equal(result1.AssertionsSuccess, testResult.AssertionsSuccess);
			Assert.Equal(result1.ScriptCompilationSuccess, testResult.ScriptCompilationSuccess);
			Assert.Equal(result1.ResponseTime, testResult.ResponseTime);
			Assert.Equal(result1.ResponseCodeSuccess, testResult.ResponseCodeSuccess);
			Assert.Equal(result1.ActualUrl, testResult.ActualUrl);
			Assert.Equal(result1.TestUrl, testResult.Test.Url);
			Assert.Equal(result1.TestDescription, testResult.Test.Description);
		}
	}
}
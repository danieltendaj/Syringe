using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syringe.Core.Tests.Results;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public class TestFileResultFactory : ITestFileResultFactory
    {
        public TestFileRunResult Create(TestFileRunnerTaskInfo runnerInfo, bool timedOut, TimeSpan timeTaken)
        {
            TestFileRunResult result;

            if (timedOut)
            {
                // Error
                result = new TestFileRunResult
                {
                    Finished = false,
                    TimeTaken = timeTaken,
                    ErrorMessage = "The runner timed out."
                };
            }
            else
            {
                if (!string.IsNullOrEmpty(runnerInfo.Errors))
                {
                    result = new TestFileRunResult
                    {
                        Finished = false,
                        TimeTaken = timeTaken,
                        ErrorMessage = runnerInfo.Errors
                    };
                }
                else
                {
                    int failCount = runnerInfo.TestFileResults?.TotalTestsFailed ?? 0;

                    result = new TestFileRunResult
                    {
                        ResultId = runnerInfo.TestFileResults?.Id,
                        HasFailedTests = (failCount > 0),
                        ErrorMessage = string.Empty,

                        Finished = DetectIfTestHasFinished(runnerInfo),
                        TestRunFailed = DetectIfTestFailed(runnerInfo),
                        TimeTaken = GetTimeTaken(runnerInfo, timeTaken),
                        TestResults = GenerateTestResults(runnerInfo)
                    };
                }
            }

            return result;
        }

        private TimeSpan GetTimeTaken(TestFileRunnerTaskInfo testFile, TimeSpan timeTaken)
        {
            return timeTaken == TimeSpan.Zero && testFile.TestFileResults != null ? testFile.TestFileResults.TotalRunTime : timeTaken;
        }
        
        private bool DetectIfTestHasFinished(TestFileRunnerTaskInfo testFileTask)
        {
            return testFileTask.CurrentTask?.Status == TaskStatus.RanToCompletion;
        }

        private bool DetectIfTestFailed(TestFileRunnerTaskInfo testFileTask)
        {
            bool failed = false;

            switch (testFileTask.CurrentTask?.Status)
            {
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    failed = true;
                    break;
            }

            return failed;
        }

        private static IEnumerable<LightweightResult> GenerateTestResults(TestFileRunnerTaskInfo runnerInfo)
        {
            return runnerInfo?.TestFileResults?.TestResults?.Select(result => new LightweightResult
            {
                Success = result.ResultState == TestResultState.Success,
                ResultState = result.ResultState.ToString(),
                Message = result.Message,
                ExceptionMessage = result.ExceptionMessage,
                AssertionsSuccess = result.AssertionsSuccess,
                ScriptCompilationSuccess = result.ScriptCompilationSuccess,
                ResponseTime = result.ResponseTime,
                ResponseCodeSuccess = result.ResponseCodeSuccess,
                ActualUrl = result.ActualUrl,
                TestUrl = result.Test?.Url,
                TestDescription = result.Test?.Description
            }) ?? new LightweightResult[0];
        }
    }
}
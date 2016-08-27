using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    Completed = false,
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
                        Completed = false,
                        TimeTaken = timeTaken,
                        ErrorMessage = runnerInfo.Errors
                    };
                }
                else
                {
                    int failCount = runnerInfo.TestFileResults?.TestResults?.Count(x => !x.Success) ?? 0;

                    result = new TestFileRunResult
                    {
                        ResultId = runnerInfo.TestFileResults?.Id,
                        HasFailedTests = (failCount > 0),
                        ErrorMessage = string.Empty,

                        Completed = DetectIfTestCompleted(runnerInfo),
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
        
        private bool DetectIfTestCompleted(TestFileRunnerTaskInfo testFileTask)
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
            return runnerInfo.Runner?.CurrentResults?.Select(lightResult => new LightweightResult
            {
                Success = lightResult.Success,
                Message = lightResult.Message,
                ExceptionMessage = lightResult.ExceptionMessage,
                AssertionsSuccess = lightResult.AssertionsSuccess,
                ScriptCompilationSuccess = lightResult.ScriptCompilationSuccess,
                ResponseTime = lightResult.ResponseTime,
                ResponseCodeSuccess = lightResult.ResponseCodeSuccess,
                ActualUrl = lightResult.ActualUrl,
                TestUrl = lightResult.Test.Url,
                TestDescription = lightResult.Test.Description
            }) ?? new LightweightResult[0];
        }
    }
}
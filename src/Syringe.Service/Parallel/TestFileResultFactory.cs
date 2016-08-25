using System;
using System.Linq;
using System.Threading.Tasks;
using Syringe.Core.Tasks;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public class TestFileResultFactory : ITestFileResultFactory
    {
        public TestFileRunResult Create(Task<TestFileRunnerTaskInfo> testFileTask, bool timedOut, TimeSpan timeTaken)
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
                if (!string.IsNullOrEmpty(testFileTask.Result.Errors))
                {
                    result = new TestFileRunResult
                    {
                        Completed = false,
                        TimeTaken = timeTaken,
                        ErrorMessage = testFileTask.Result.Errors
                    };
                }
                else
                {
                    TestFileRunnerTaskInfo testFile = testFileTask.Result;
                    int failCount = testFile.Runner.CurrentResults.Count(x => !x.Success);

                    result = new TestFileRunResult
                    {
                        ResultId = testFile.TestFileResults?.Id,
                        Completed = DetectIfTestCompleted(testFileTask),
                        Failed = DetectIfTestFailed(testFileTask),
                        TimeTaken = GetTimeTaken(testFile, timeTaken),
                        HasFailedTests = (failCount > 0),
                        ErrorMessage = GetErrorMessage(testFileTask),
                        TestResults = testFile.Runner.CurrentResults.Select(lightResult => new LightweightResult()
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
                        })
                    };
                }
            }

            return result;
        }

        private TimeSpan GetTimeTaken(TestFileRunnerTaskInfo testFile, TimeSpan timeTaken)
        {
            return timeTaken == TimeSpan.Zero && testFile.TestFileResults != null ? testFile.TestFileResults.TotalRunTime : timeTaken;
        }

        private string GetErrorMessage(Task<TestFileRunnerTaskInfo> testFileTask)
        {
            if (testFileTask.Exception != null)
            {
                return testFileTask.Exception.ToString();
            }

            return string.Empty;
        }

        private bool DetectIfTestCompleted(Task<TestFileRunnerTaskInfo> testFileTask)
        {
            return testFileTask.Result.CurrentTask?.Status == TaskStatus.RanToCompletion;
        }

        private bool DetectIfTestFailed(Task<TestFileRunnerTaskInfo> testFileTask)
        {
            bool failed = false;

            switch (testFileTask.Result.CurrentTask?.Status)
            {
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    failed = true;
                    break;
            }

            return failed;
        }
    }
}
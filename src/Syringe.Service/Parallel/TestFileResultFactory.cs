using System;
using System.Linq;
using System.Threading.Tasks;
using Syringe.Core.Tasks;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public class TestFileResultFactory : ITestFileResultFactory
    {
        public TestFileRunResult Create(TestFileRunnerTaskInfo testFileRunnerInfo, bool timedOut, TimeSpan timeTaken)
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
                if (!string.IsNullOrEmpty(testFileRunnerInfo.Errors))
                {
                    result = new TestFileRunResult
                    {
                        Completed = false,
                        TimeTaken = timeTaken,
                        ErrorMessage = testFileRunnerInfo.Errors
                    };
                }
                else
                {
                    TestFileRunnerTaskInfo testFile = testFileRunnerInfo;
                    int failCount = testFile.Runner.CurrentResults.Count(x => !x.Success);

                    result = new TestFileRunResult
                    {
                        ResultId = testFile.TestFileResults?.Id,
                        Completed = DetectIfTestCompleted(testFileRunnerInfo),
                        Failed = DetectIfTestFailed(testFileRunnerInfo),
                        TimeTaken = GetTimeTaken(testFile, timeTaken),
                        HasFailedTests = (failCount > 0),
                        ErrorMessage = string.Empty,
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
    }
}
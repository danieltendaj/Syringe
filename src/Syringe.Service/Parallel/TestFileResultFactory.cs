using System;
using System.Linq;
using System.Threading.Tasks;
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
                    int failCount = testFileTask.Result.Runner.CurrentResults.Count(x => !x.Success);

                    result = new TestFileRunResult
                    {
                        ResultId = testFileTask.Result.TestFileResults.Id,
                        Completed = true,
                        TimeTaken = timeTaken,
                        HasFailedTests = (failCount > 0),
                        ErrorMessage = "",
                        TestResults = testFileTask.Result.Runner.CurrentResults.Select(lightResult => new LightweightResult()
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
    }
}
using System;
using System.Threading.Tasks;
using Syringe.Service.Api;

namespace Syringe.Service.Parallel
{
    public interface ITestFileResultFactory
    {
        TestFileRunResult Create(Task<TestFileRunnerTaskInfo> testFileTask, bool timedOut, TimeSpan timeTaken);
    }
}
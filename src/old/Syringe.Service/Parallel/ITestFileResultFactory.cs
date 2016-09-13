using System;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public interface ITestFileResultFactory
    {
        TestFileRunResult Create(TestFileRunnerTaskInfo runnerInfo, bool timedOut, TimeSpan timeTaken);
    }
}
using System;
using System.Threading.Tasks;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
    public interface ITestFileResultFactory
    {
        TestFileRunResult Create(TestFileRunnerTaskInfo testFileRunnerInfo, bool timedOut, TimeSpan timeTaken);
    }
}
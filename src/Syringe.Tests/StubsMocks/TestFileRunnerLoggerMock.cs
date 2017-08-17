using System;
using Syringe.Core.Runner.Logging;

namespace Syringe.Tests.StubsMocks
{
    public class TestFileRunnerLoggerMock : ITestFileRunnerLogger
    {
        public string GetLog()
        {
            return "";
        }

        public void AppendTextLine(string text)
        {
        }

        public void Write(string message, params object[] args)
        {
        }

        public void WriteLine(string message, params object[] args)
        {
        }

        public void WriteLine(Exception ex, string message, params object[] args)
        {
        }
    }
}
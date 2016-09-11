using System;

namespace Syringe.Core.Runner.Logging
{
    /// <summary>
    /// A logger for the test runner - the output is viewed by the user (the logging isn't
    /// just for diagnostics).
    /// </summary>
    public interface ITestFileRunnerLogger
    {
        string GetLog();
        void AppendTextLine(string text);
        void Write(string message, params object[] args);
        void WriteLine(string message, params object[] args);
        void WriteLine(Exception ex, string message, params object[] args);
    }
}
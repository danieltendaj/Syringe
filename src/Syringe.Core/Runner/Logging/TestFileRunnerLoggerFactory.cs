namespace Syringe.Core.Runner.Logging
{
    public class TestFileRunnerLoggerFactory : ITestFileRunnerLoggerFactory
    {
        public ITestFileRunnerLogger CreateLogger()
        {
            return new TestFileRunnerLogger();
        }
    }
}
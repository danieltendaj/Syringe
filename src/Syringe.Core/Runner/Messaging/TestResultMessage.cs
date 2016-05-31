using Syringe.Core.Tests.Results;

namespace Syringe.Core.Runner.Messaging
{
    public class TestResultMessage : IMessage
    {
        public MessageType MessageType => MessageType.TestCompletionResult;
        public TestResult TestResult { get; set; }
    }
}
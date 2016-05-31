using System;

namespace Syringe.Core.Runner.Messaging
{
    public class TestFileGuidMessage : IMessage
    {
        public MessageType MessageType => MessageType.TestFileGuid;
        public Guid ResultId { get; set; }
    }
}
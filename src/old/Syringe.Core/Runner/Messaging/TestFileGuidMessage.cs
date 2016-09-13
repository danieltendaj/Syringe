using System;

namespace Syringe.Core.Runner.Messaging
{
    public class TestFileGuidMessage : IMessage
    {
        public Guid ResultId { get; set; }
    }
}
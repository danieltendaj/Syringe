namespace Syringe.Core.Runner.Messaging
{
    public interface IMessage
    {
        MessageType MessageType { get; }
    }
}
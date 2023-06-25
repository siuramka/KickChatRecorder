namespace KickChatRecorder.Contracts
{
    public interface IProducer
    {
        Task Run(IKickChatClient client);
    }
}

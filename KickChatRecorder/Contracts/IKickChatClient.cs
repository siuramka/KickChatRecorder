using System.Net.WebSockets;

namespace KickChatRecorder.Contracts
{
    public interface IKickChatClient
    {
        Task ConnectAsync();
        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token);
        Task CloseAsync();
    }
}

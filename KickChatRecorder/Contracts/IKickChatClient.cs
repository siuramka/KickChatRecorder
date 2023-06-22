using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Contracts
{
    public interface IKickChatClient
    {
        Task ConnectAsync();
        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token);
        Task CloseAsync();
    }
}

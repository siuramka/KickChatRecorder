using KickChatRecorder.Contracts;
using KickChatRecorder.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KickChatRecorder.Test
{
    public class TestChatClient : IKickChatClient
    {
        private static readonly string _connectionString = "ws://localhost:4444";
        private KickChatClientConfiguration _kickChatClientConfiguration;
        private ClientWebSocket _socketClient = null;

        public TestChatClient(KickChatClientConfiguration kickChatClientConfiguration)
        {
            _socketClient = new ClientWebSocket();
            _kickChatClientConfiguration = kickChatClientConfiguration;
        }

        public async Task ConnectAsync()
        {

            try
            {
                await _socketClient.ConnectAsync(new Uri(_connectionString), CancellationToken.None);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR - {ex.Message}");
            }
        }

        public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token)
        {
            return await _socketClient.ReceiveAsync(buffer, token);
        }

        public void Dispose()
        {
            _socketClient.Dispose();
        }

        public async Task CloseAsync()
        {
            await _socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
    }
}

using KickChatRecorder.Contracts;
using KickChatRecorder.Models.Config;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace KickChatRecorder.Client
{
    public class KickChatClient : IKickChatClientWithSend, IDisposable
    {
        /// <summary>
        /// Pusher wss connection. Protocol version must be higher than v3
        /// </summary>
        private static readonly string _connectionString = "wss://ws-us2.pusher.com/app/eb1d5f283081a78b932c?protocol=7&client=js&version=7.4.0&flash=false";
        private KickChatClientConfiguration _kickChatClientConfiguration;
        private ClientWebSocket _socketClient = null;

        public KickChatClient(KickChatClientConfiguration kickChatClientConfiguration)
        {
            _socketClient = new ClientWebSocket();
            _kickChatClientConfiguration = kickChatClientConfiguration;
        }

        /// <summary>
        /// Connects to the Pusher wss and sends a pusher event subscribtion event to listen to wss messages of _kickChatClientConfiguration.ChatroomId 
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {

            var connectionData = JsonSerializer.Serialize(_kickChatClientConfiguration.ConnectionRequest);

            try
            {
                await _socketClient.ConnectAsync(new Uri(_connectionString), CancellationToken.None);
                await Send(connectionData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR - {ex.Message}");
            }
        }
        public async Task CloseAsync()
        {
            await _socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public async Task Send(string data)
        {
            await _socketClient.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token)
        {
            return await _socketClient.ReceiveAsync(buffer, token);
        }

        public void Dispose()
        {
            _socketClient.Dispose();
        }
    }
}

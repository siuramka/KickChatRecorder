using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace KickChatRecorder
{
    public class ConnectionRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("auth")]
        public string Auth { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
    }

    internal class Program
    {
        private static readonly string Connection = "wss://ws-us2.pusher.com/app/eb1d5f283081a78b932c?protocol=7&client=js&version=7.4.0&flash=false";

        static async Task Main(string[] args)
        {

            ConnectionRequest connectionRequest = new ConnectionRequest();
            connectionRequest.Event = "pusher:subscribe";
            connectionRequest.Data = new Data();
            connectionRequest.Data.Auth = "";
            connectionRequest.Data.Channel = "chatrooms.7022952.v2";
            var connectionData = JsonSerializer.Serialize(connectionRequest);

            using (var socket = new ClientWebSocket())
            {
                try
                {
                    await socket.ConnectAsync(new Uri(Connection), CancellationToken.None);

                    await Send(socket, connectionData);
                    await Receive(socket);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - {ex.Message}");
                }
            }

        }

        static async Task Send(ClientWebSocket socket, string data)
        {
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            while (true)
            {
                WebSocketReceiveResult result;
                while (true)
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);

                    if (result.EndOfMessage)
                        break;
                }

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                ms.Seek(0, SeekOrigin.Begin);
                Console.WriteLine(await reader.ReadToEndAsync());
            }

        }
    }
}
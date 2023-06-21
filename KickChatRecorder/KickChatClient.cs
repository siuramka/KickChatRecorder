using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KickChatRecorder
{
    public class KickChatClient
    {
        /// <summary>
        /// Pusher wss connection. Protocol version must be higher than v3
        /// </summary>
        private static readonly string _connectionString = "wss://ws-us2.pusher.com/app/eb1d5f283081a78b932c?protocol=7&client=js&version=7.4.0&flash=false";
        private readonly KickChatClientConfiguration _kickChatClientConfiguration;

        public KickChatClient(KickChatClientConfiguration kickChatClientConfiguration)
        {
            _kickChatClientConfiguration = kickChatClientConfiguration;
        }
        /// <summary>
        /// Connects to the Pusher wss and sends a pusher event subscribtion event to listen to wss messages of _kickChatClientConfiguration.ChatroomId 
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            ConnectionRequest connectionRequest = new ConnectionRequest();
            connectionRequest.Event = "pusher:subscribe";
            connectionRequest.Data = new Data();
            connectionRequest.Data.Auth = "";
            connectionRequest.Data.Channel = $"chatrooms.{_kickChatClientConfiguration.ChatroomId}.v2";
            var connectionData = JsonSerializer.Serialize(connectionRequest);

            using (var socket = new ClientWebSocket())
            {
                try
                {
                    await socket.ConnectAsync(new Uri(_connectionString), CancellationToken.None);

                    await Send(socket, connectionData);
                    await Receive(socket);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Send a message to the websocket connection
        /// 
        /// Private since only reading messages
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task Send(ClientWebSocket socket, string data)
        {
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task Receive(ClientWebSocket socket)
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

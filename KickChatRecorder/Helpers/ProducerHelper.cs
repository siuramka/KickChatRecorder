using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KickChatRecorder.Helpers
{
    public static class ProducerHelper
    {
        /// <summary>
        /// Deserializes string json message data into a MessageData object
        /// </summary>
        /// <param name="jsonData">JSON data containing message info</param>
        /// <returns>Deserialized jsonData MessasgeData object</returns>
        public static MessageData GetMessageFromString(string jsonData)
        {
            var chatDataTemp = JsonSerializer.Deserialize<TempMessageData>(jsonData);
            chatDataTemp.Channel = GetChannelId(chatDataTemp);

            var chatInfoTemp = JsonSerializer.Deserialize<ChatInfo>(chatDataTemp.Data);
            MessageData messageData = new MessageData(chatDataTemp, chatInfoTemp);

            return messageData;
        }

        /// <summary>
        /// Gets the channel ID from a string - channels.{id}.v2 => id
        /// </summary>
        /// <param name="tempMessage">Temporary deserialized MessageData</param>
        /// <returns>channel id</returns>
        private static string GetChannelId(TempMessageData tempMessage)
        {
            return tempMessage.Channel.Split('.')[1];
        }

        public static async Task ReccuringSendPing(IKickChatClientWithSend client, int seconds, CancellationToken token, CancellationToken timeoutToken)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if(timeoutToken.IsCancellationRequested)
                    {
                        await client.Send(JsonSerializer.Serialize(new Ping()));
                    }
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("cancl");
            }
        }
    }
}

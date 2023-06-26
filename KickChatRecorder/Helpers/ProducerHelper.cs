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
        public static MessageData GetMessageFromString(string data)
        {
            var chatDataTemp = JsonSerializer.Deserialize<TempMessageData>(data);
            chatDataTemp.Channel = GetChannelId(chatDataTemp);

            var chatInfoTemp = JsonSerializer.Deserialize<ChatInfo>(chatDataTemp.Data);
            MessageData messageData = new MessageData(chatDataTemp, chatInfoTemp);

            return messageData;
        }
        private static string GetChannelId(TempMessageData tempMessage)
        {
            return tempMessage.Channel.Split('.')[1];
        }
    }
}

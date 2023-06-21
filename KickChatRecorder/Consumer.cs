using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KickChatRecorder
{

    public class Consumer
    {
        private ChannelReader<string> _reader;
        public Consumer(ChannelReader<string> reader)
        {
            _reader = reader;
            Task.WaitAll(this.Run());
        }
        public async Task Run()
        {
            while(await _reader.WaitToReadAsync())
            {
                var item = await _reader.ReadAsync();
                try
                {
                    var chatDataTemp = JsonSerializer.Deserialize<TempMessageData>(item);
                    var data = JsonSerializer.Deserialize<ChatInfo>(chatDataTemp.Data);
                    MessageData messageData = new MessageData();
                    messageData.Channel = chatDataTemp.Channel;
                    messageData.Data = data;
                    messageData.Event = chatDataTemp.Event;
                    string fileName = $"{messageData.Channel}.txt";
                    await File.AppendAllTextAsync(fileName, messageData.ToString() + "\n");

                } catch(Exception ex)
                {
                    Console.WriteLine("Failed to parse/write message:" + ex);
                }
            }
        }
    }
}

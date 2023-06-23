using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KickChatRecorder
{

    public class Consumer
    {
        private ChannelReader<string> _reader;
        private ChannelWriter<List<MessageData>> _fileStreamWriter;
        public Consumer(ChannelReader<string> reader, ChannelWriter<List<MessageData>> fileStreamWriter)
        {
            _reader = reader;
            _fileStreamWriter = fileStreamWriter;
            Task.WaitAll(this.Run());
        }
        public async Task Run()
        {
            List<MessageData> batch = new List<MessageData>();

            while (await _reader.WaitToReadAsync())
            {
                var item = await _reader.ReadAsync();

                try
                {
                    var chatDataTemp = JsonSerializer.Deserialize<TempMessageData>(item);
                    var chatInfoTemp = JsonSerializer.Deserialize<ChatInfo>(chatDataTemp.Data);
                    MessageData messageData = new MessageData(chatDataTemp, chatInfoTemp);
                    string fileName = $"{messageData.Channel}.txt";
                    Console.WriteLine(item + "\n");
                    //File.AppendAllText(fileName, messageData.ToString() + "\n");
                    if (batch.Count < 10)
                    {
                        batch.Add(messageData);
                    }
                    else if (batch.Count >= 10)
                    {
                        await _fileStreamWriter.WriteAsync(batch);
                        batch = new List<MessageData>();
                    }
        
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to parse/write message:" + ex);
                    Console.WriteLine(item);
                    
                }
            }
            
        }
    }
}

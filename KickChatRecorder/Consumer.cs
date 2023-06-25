using KickChatRecorder.Models;
using KickChatRecorder.Service;
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
        private CassandraService _cassandraService;
        public Consumer(ChannelReader<string> reader, CassandraService cassandraService)
        {
            _reader = reader;
            _cassandraService = cassandraService;
            Task.WaitAll(this.Run());
        }
        public async Task Run()
        {
            while (await _reader.WaitToReadAsync())
            {
                var item = await _reader.ReadAsync();

                try
                {
                    var chatDataTemp = JsonSerializer.Deserialize<TempMessageData>(item);
                    var chatInfoTemp = JsonSerializer.Deserialize<ChatInfo>(chatDataTemp.Data);
                    MessageData messageData = new MessageData(chatDataTemp, chatInfoTemp);
                    try
                    {
                        await _cassandraService.InsertChannel(messageData);
                        await _cassandraService.InsertMessage(messageData);
                    }
                    catch
                    {
                        Console.WriteLine("FAIL");
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

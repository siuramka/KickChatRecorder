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
        private ChannelReader<MessageData> _reader;
        private CassandraService _cassandraService;
        public Consumer(ChannelReader<MessageData> reader, CassandraService cassandraService)
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

                    await _cassandraService.InsertChannel(item);
                    await _cassandraService.InsertMessage(item);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to insert to database" + ex);
                    Console.WriteLine(item);
                    
                }
            }
            
        }
    }
}

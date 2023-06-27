using KickChatRecorder.Models;
using KickChatRecorder.Service;
using System.Threading.Channels;

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
            try
            {
                while (await _reader.WaitToReadAsync())
                {
                    var item = await _reader.ReadAsync();
             
                    try
                    {
                        await _cassandraService.InsertMessage(item);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to insert to database" + ex);
                        Console.WriteLine(item);
                    
                    }
                }

            }
            catch (ChannelClosedException)
            {
                Console.WriteLine("Channel has been closed per request...");
            }
        }
    }
}

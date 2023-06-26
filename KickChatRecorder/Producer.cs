using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using System.Text;
using KickChatRecorder.Helpers;
using System.Threading.Channels;

namespace KickChatRecorder
{
    public class Producer
    {
        private ChannelWriter<MessageData> _writer;
        public Producer(ChannelWriter<MessageData> writer, IKickChatClient client, CancellationToken token)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Task.WaitAll(this.Run(client, token));
        }
        public async Task Run(IKickChatClient client, CancellationToken token)
        {
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            var buffer = new byte[1 * 1024];
            while (!token.IsCancellationRequested) // only checks if receives message from ws, but gets pinged every 
            {
                try
                {
                    var result = await client.ReceiveAsync(buffer, CancellationToken.None);

                    ms.Write(buffer, 0, result.Count);
                    ms.Seek(0, SeekOrigin.Begin);

                    var data = await reader.ReadToEndAsync();

                    // write only chat messages
                    if (data.Contains(KickChatEvents.ChatMessageEvent))
                    {
                        await _writer.WriteAsync(ProducerHelper.GetMessageFromString(data));
                    }
                    else
                    {
                        Console.WriteLine(data);
                    }

                    ms.SetLength(0); // Clear the MemoryStream
                    ms.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to produce item " + ex);
                }
            }
            if (_writer.TryComplete())
            {
                Console.WriteLine("Writer closed...");
            }
        }
    }
}

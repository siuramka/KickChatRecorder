using KickChatRecorder.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KickChatRecorder
{
    public class Producer
    {
        private ChannelWriter<string> _writer;
        public Producer(ChannelWriter<string> writer, IKickChatClient client)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Task.WaitAll(this.Run(client));
        }
        public async Task Run(IKickChatClient client)
        {
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            var buffer = new byte[1 * 1024];
            while (await _writer.WaitToWriteAsync())
            {
                try
                {
                    //var result = await client.ReceiveAsync(buffer, CancellationToken.None);
                    var receiveTask = client.ReceiveAsync(buffer, CancellationToken.None);
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2)); // Adjust the timeout duration as needed

                    var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        Console.WriteLine("DONE");
                        await client.CloseAsync();
                        _writer.Complete();
                    }

                    var result = receiveTask.Result;

                    ms.Write(buffer, 0, result.Count);
                    ms.Seek(0, SeekOrigin.Begin);

                    var data = await reader.ReadToEndAsync();
                    await _writer.WriteAsync(data);
                    ms.SetLength(0); // Clear the MemoryStream
                    ms.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to produce item " + ex);
                }
            }
        }
    }
}

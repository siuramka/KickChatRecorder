using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using System.Text;
using KickChatRecorder.Helpers;
using System.Threading.Channels;
using System.Diagnostics;
using System;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Runtime.CompilerServices;

namespace KickChatRecorder
{
    public class Producer
    {
        private ChannelWriter<MessageData> _writer;
        private CancellationToken _timeoutToken;
        public Producer(ChannelWriter<MessageData> writer, IKickChatClientWithSend client, CancellationToken token)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

            var ttas = ProducerHelper.ReccuringSendPing(client, 5, token, _timeoutToken);
            var rrtas = this.Run(client, token);
                Task.WaitAll(rrtas, ttas);
  
        }

        public async Task Run(IKickChatClientWithSend client, CancellationToken token)
        {
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            var buffer = new byte[1 * 1024];
            while (!token.IsCancellationRequested) // only checks if receives message from ws, but gets pinged every 
            {
                try
                {
                    var result = await client.ReceiveAsync(buffer, token);

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

                    _timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

                    ms.SetLength(0); // Clear the MemoryStream
                    ms.Seek(0, SeekOrigin.Begin);

                }
                catch (ChannelClosedException)
                { }
                catch (TaskCanceledException)
                { }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to produce item " + ex);
                }
            }
            if (_writer.TryComplete())
            {
                Console.WriteLine("Writer closed...");
            }
            else
            {
                Console.WriteLine("Writer already completed!");

            }
        }
    }
}

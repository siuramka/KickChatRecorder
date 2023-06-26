﻿using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using System.Text;
using KickChatRecorder.Helpers;
using System.Threading.Channels;

namespace KickChatRecorder
{
    public class Producer : IProducer
    {
        private ChannelWriter<MessageData> _writer;
        public Producer(ChannelWriter<MessageData> writer, IKickChatClient client)
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
                    var result = await client.ReceiveAsync(buffer, CancellationToken.None);

                    ms.Write(buffer, 0, result.Count);
                    ms.Seek(0, SeekOrigin.Begin);

                    var data = await reader.ReadToEndAsync();
    
                    // write only chat messages
                    if(data.Contains(KickChatEvents.ChatMessageEvent))
                    {
                        await _writer.WriteAsync(ProducerHelper.GetMessageFromString(data));
                    } else
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
        }
    }
}

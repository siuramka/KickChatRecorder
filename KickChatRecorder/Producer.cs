using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KickChatRecorder
{
    public class Producer
    {
        private ChannelWriter<string> _writer;
        public Producer(ChannelWriter<string> writer, KickChatClient client)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Task.WaitAll(this.Run(client));
        }
        public async Task Run(KickChatClient client)
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            while (await _writer.WaitToWriteAsync())
            {
                var result = await client.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
                ms.Seek(0, SeekOrigin.Begin);
                await _writer.WriteAsync(await reader.ReadToEndAsync());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder
{
    public class ChatRecorder
    {
        private List<KickChatClient> _clients;

        public ChatRecorder(List<KickChatClient> clients)
        {
            _clients = clients;
        }

        public async Task RecordChats(KickChatClient client)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            while(true)
            {
                var result = await client.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
                ms.Seek(0, SeekOrigin.Begin);
                Console.WriteLine(await reader.ReadToEndAsync());
            }
        }
    }
}

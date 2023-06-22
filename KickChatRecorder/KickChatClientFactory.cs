using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder
{
    public class KickChatClientFactory
    {
        public KickChatClientFactory() { }

        public KickChatClient CreateClient(string chatroomId)
        {
            KickChatClientConfiguration config = new KickChatClientConfiguration(chatroomId);
            KickChatClient client = new KickChatClient(config);
            return client;
        }
    }
}

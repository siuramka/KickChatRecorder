using KickChatRecorder.Contracts;
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

        public IKickChatClient CreateClient(string chatroomId)
        {
            KickChatClientConfiguration config = new KickChatClientConfiguration(chatroomId);
            IKickChatClient client = new KickChatClient(config);
            return client;
        }

        public IKickChatClient CreateTestClient(string chatroomId)
        {
            KickChatClientConfiguration config = new KickChatClientConfiguration(chatroomId);
            IKickChatClient client = new TestChatClient(config);
            return client;
        }
    }
}

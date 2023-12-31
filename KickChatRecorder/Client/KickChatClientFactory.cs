﻿using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using KickChatRecorder.Test;

namespace KickChatRecorder.Client
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
        public IKickChatClientWithSend CreateClientSendable(string chatroomId)
        {
            KickChatClientConfiguration config = new KickChatClientConfiguration(chatroomId);
            IKickChatClientWithSend client = new KickChatClient(config);
            return client;
        }

        public IKickChatClientWithSend CreateTestClientSendable(string chatroomId)
        {
            KickChatClientConfiguration config = new KickChatClientConfiguration(chatroomId);
            IKickChatClientWithSend client = new TestChatClient(config);
            return client;
        }
    }
}

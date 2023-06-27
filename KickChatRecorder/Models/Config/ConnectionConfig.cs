using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Models.Config
{
    public class ConnectionConfig
    {
        public ConnectionRequest ConnectionRequest { get; set; }
        public ConnectionConfig(string pusherEvent, string auth, string chatroomId)
        {
            ConnectionRequest = new ConnectionRequest(chatroomId);
            ConnectionRequest.Event = pusherEvent;
            ConnectionRequest.Data = new Data();
            ConnectionRequest.Data.Channel = $"chatrooms.{chatroomId}.v2";
            ConnectionRequest.Data.Auth = auth;
        }
    }
}

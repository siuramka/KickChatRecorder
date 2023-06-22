using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Models
{
    public class KickChatClientConfiguration
    {
        public string ChatroomId { get; set; }
        public ConnectionRequest ConnectionRequest { get; set; }
        public KickChatClientConfiguration(string chatroomId) {
            ChatroomId = chatroomId;
            ConnectionRequest = new ConnectionRequest(chatroomId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Models.DTO
{
    public class Messages
    {
        public string channel_id { get; set; }
        public DateTime message_idtime { get; set; }
        public string message_creatorusername { get; set; }
        public string message_text { get; set; }
        public Guid message_uuid { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KickChatRecorder.Models
{
    public class ConnectionRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("auth")]
        public string Auth { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
    }
}

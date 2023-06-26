using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KickChatRecorder.Models
{
    /// <summary>
    /// Tempory class to deserialization as the original strint for deserialization doesnt follow C# JSON encoding format.
    /// </summary>
    public class TempMessageData
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("channel")]
        public string Channel { get; set; }
    }
}

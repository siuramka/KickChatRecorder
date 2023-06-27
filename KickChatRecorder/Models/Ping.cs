using System.Text.Json.Serialization;

namespace KickChatRecorder.Models
{
    public class Ping
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("data")]
        public NullData Data { get; set; }
        public Ping()
        {
            Event = "pusher:ping";
            Data = new NullData();
        }
    }

    public class NullData
    {
    }
}

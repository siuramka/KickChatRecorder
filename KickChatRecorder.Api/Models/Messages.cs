namespace KickChatRecorder.Api.Models
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

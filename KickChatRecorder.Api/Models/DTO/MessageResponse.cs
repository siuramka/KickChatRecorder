using Cassandra.Mapping.Attributes;

namespace KickChatRecorder.Api.Models.DTO
{
    public class MessageResponse
    {
        [Column("message_uuid")]
        public Guid Id { get; set; }
        [Column("message_idtime")]
        public DateTime CreatedAt { get; set; }
        [Column("message_creatorusername")]
        public string Username { get; set; }
        [Column("message_text")]
        public string Text { get; set; }
    }
}

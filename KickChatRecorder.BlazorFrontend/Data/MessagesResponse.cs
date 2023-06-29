using System.ComponentModel.DataAnnotations.Schema;

namespace KickChatRecorder.BlazorFrontend.Data
{
    public class MessagesResponse
    {
        public List<Message> messages { get; set; }
        public string PagingInfo { get; set; }
    }
    public class Message
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace KickChatRecorder.Api.Models.DTO
{
    internal class MessagesResponse
    {
        public List<MessageResponse> messages { get; set; }
        public string PagingInfo { get; set; }
    }
}

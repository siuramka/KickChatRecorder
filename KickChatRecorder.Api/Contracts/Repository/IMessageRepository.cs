using KickChatRecorder.Api.Models;
using KickChatRecorder.Api.Paging;

namespace KickChatRecorder.Api.Contracts.Repository
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Messages>> GetMessagesAsync(MessageParameters messageParameters);
    }
}

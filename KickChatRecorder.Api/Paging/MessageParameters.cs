namespace KickChatRecorder.Api.Paging
{
    public class MessageParameters : RequestParameters
    {
        public string ChannelName { get; set; }
        public string? Username { get; set; }
    }
}

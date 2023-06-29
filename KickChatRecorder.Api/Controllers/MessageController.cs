using Cassandra.Mapping;
using KickChatRecorder.Api.Models;
using KickChatRecorder.Api.Models.DTO;
using KickChatRecorder.Api.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Text;

namespace KickChatRecorder.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private IMapper _mapper;

        public MessageController(Cassandra.ISession session)
        {
            _mapper = new Mapper(session);
        }

        [HttpGet]
        [Route("getmessages")]
        public async Task<IActionResult> GetMessages([FromQuery] MessageParameters messageParameters)
        {
            if(messageParameters.PageSize > 100)
            {
                messageParameters.PageSize = 100;
            }

            IPage<MessageResponse> message = null;

            var channel = _mapper.Fetch<Channels>(@$"select * from store.channels where channel_name='{messageParameters.ChannelName}'").FirstOrDefault();

            string query = $@"select * from store.messages where channel_id='{channel.channel_id}'";

            if (messageParameters.Username != null && messageParameters.Username != "")
            {
                query = @$"select * from store.messages where message_creatorusername='{messageParameters.Username}' and channel_id='{channel.channel_id}'";
            }

            if (messageParameters.PagingInfo != null && messageParameters.PagingInfo != "")
            {
                //convert pageinfo sent from the client to byte[] format
                byte[] pagingbytes = HexaDecimalStringToByteArray(messageParameters.PagingInfo);
                message = await _mapper.FetchPageAsync<MessageResponse>
                          (Cql.New(query)
                          .WithOptions(opt => opt.SetPageSize(messageParameters.PageSize) //set pagesize. How many data display in each request
                          .SetPagingState(pagingbytes))); //pagingbytes to request the current page
            }
            else
            {
                //Default query if there is no pagination info
                message = await _mapper.FetchPageAsync<MessageResponse>
                            (Cql.New(query)
                            .WithOptions(opt => opt.SetPageSize(messageParameters.PageSize)));
            }
            string page_result = null;
            if (message.PagingState != null)
            {
                //convert to string format from byte[] for the client to be used in next request
                page_result = ByteArrayToHexaDecimalString(message.PagingState);
            }
            MessagesResponse messagesResponse = new MessagesResponse();
            messagesResponse.messages = message.ToList();
            messagesResponse.PagingInfo = page_result;
  

            return Ok(messagesResponse);
        }
        /// <summary>
        /// Converts ByteArray To String
        /// For some reason its the only this is the only encoding working with mapping paging
        /// </summary>
        public static string ByteArrayToHexaDecimalString(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) { stringBuilder.AppendFormat("{0:x2}", b); }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// Converts string To ByteArray
        /// For some reason its the only this is the only encoding working with mapping paging
        /// </summary>
        public static byte[] HexaDecimalStringToByteArray(string hexaDecimalString)
        {
            int NumberChars = hexaDecimalString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexaDecimalString.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
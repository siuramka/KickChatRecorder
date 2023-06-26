using Cassandra;
using Cassandra.Mapping;
using KickChatRecorder.Models;
using KickChatRecorder.Models.DTO;
using System.Security.AccessControl;

namespace KickChatRecorder.Service
{
    public class CassandraService
    {
        //private ISession _session;
        private IMapper _mapper;

        public CassandraService(ISession session)
        {
            //_session = session ?? throw new ArgumentNullException(nameof(session));
            _mapper = new Mapper(session);
        }

        //execute is async by nature...
        //see https://github.com/search?q=repo%3Adatastax%2Fcsharp-driver+execute&type=code
        //also https://www.datastax.com/blog/asynchronous-queries-java-driver (java)

        /// Mapper uses a lot of cpu usage - 20% of 100% total used - per performance profiler
        public async Task<AppliedInfo<Channels>> InsertChannel(MessageData message)
        {
            var channel = new Channels
            {
                channel_id = message.Channel,
                channel_name = message.Channel
            };

            var result = await _mapper.InsertIfNotExistsAsync(channel);

            return result;
        }

        public async Task InsertMessage(MessageData message)
        {
            Guid messageuuid = Guid.NewGuid();
            var messageInsert = new Messages
            {
                channel_id = message.Channel,
                message_idtime = message.Data.CreatedAt,
                message_text = message.Data.Content,
                message_creatorusername = message.Data.Sender.Username,
                message_uuid = messageuuid
            };

            await _mapper.InsertAsync(messageInsert);
        }
        public async Task<IEnumerable<Channels>> GetChannels()
        {
            return await _mapper.FetchAsync<Channels>();
        }
    }
}

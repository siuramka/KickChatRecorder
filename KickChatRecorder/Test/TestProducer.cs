using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using System.Text;
using KickChatRecorder.Helpers;
using System.Threading.Channels;
using KickChatRecorder.Models.Config;
using System.Text.Json;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;

namespace KickChatRecorder
{
    public class TestProducer
    {
        private ChannelWriter<MessageData> _writer;
        private CancellationToken _token;
        public TestProducer(ChannelWriter<MessageData> writer, IKickChatClientWithSend client, CancellationToken token)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _token = token;

            var producerTask = this.Run(client);
            try
            {
                Task.WaitAll(producerTask);

            } catch
            {
                Task.Run(client.CloseAsync);
            }
        }

        public async Task Run(IKickChatClientWithSend client)
        {
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            var buffer = new byte[1 * 1024];
            try
            {
                while (!_token.IsCancellationRequested)
                {
                    var result = await client.ReceiveAsync(buffer, _token);

                    ms.Write(buffer, 0, result.Count);
                    ms.Seek(0, SeekOrigin.Begin);

                    var data = await reader.ReadToEndAsync();
                
                    try
                    {
                        await _writer.WriteAsync(ProducerHelper.GetMessageFromString(data));
                    }
                    catch { }


                    ms.SetLength(0); // Clear the MemoryStream
                    ms.Seek(0, SeekOrigin.Begin);
                }
            }
            catch
            {
                _token = new CancellationTokenSource(1).Token;
            }

        }

        /// <summary>
        /// 
        /// Method used to send a ping/ping to websocket client
        /// 
        /// </summary>
        /// <param name="client">KickChatClient that can send to websocket</param>
        /// <param name="timeoutTime">At how often send a ping </param>
        /// <param name="token">Main cancellation token for closing all channels,producers, consumers etc</param>
        /// <param name="timeoutToken">whenether or not it has passed X ammount of time since last message was received</param>
        /// <returns>completed task</returns>
        /// pingTask 
    }
}

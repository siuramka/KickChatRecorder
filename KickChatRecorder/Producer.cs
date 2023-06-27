using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using System.Text;
using KickChatRecorder.Helpers;
using System.Threading.Channels;
using KickChatRecorder.Models.Config;
using System.Text.Json;
using System.Linq.Expressions;

namespace KickChatRecorder
{
    public class Producer
    {
        private ChannelWriter<MessageData> _writer;
        private static CancellationToken _timeoutToken;
        public Producer(ChannelWriter<MessageData> writer, IKickChatClientWithSend client, CancellationToken token)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(PusherConfig.ActivityTimeoutSeconds)).Token; // create initial timeout

            var pingTask = Task.Run(async () =>
            {            
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (_timeoutToken.IsCancellationRequested)
                        {
                            await client.Send(JsonSerializer.Serialize(new Ping()));
                        }
                        await Task.Delay(TimeSpan.FromSeconds(PusherConfig.ActivityTimeoutSeconds), token);
                    }
                }
                catch (TaskCanceledException) // method will throw canceled because if cancellation token but I want it completed so it will eat the exception and complete task instead
                { }

            });

            var producerTask = this.Run(client, token);

            Task.WaitAll(producerTask, pingTask);// will wait till ping is send even if theres a token cancellation request
        }

        public async Task Run(IKickChatClientWithSend client, CancellationToken token)
        {
            var ms = new MemoryStream();
            var reader = new StreamReader(ms, Encoding.UTF8);
            var buffer = new byte[1 * 1024];

            while (!token.IsCancellationRequested) // only checks if receives message from ws, but gets pinged every - not any more since i added pinging heheeheh
            {
                try
                {
                    var result = await client.ReceiveAsync(buffer, token);

                    ms.Write(buffer, 0, result.Count);
                    ms.Seek(0, SeekOrigin.Begin);

                    var data = await reader.ReadToEndAsync();

                    var kickEvent = PusherHelper.GetPusherEvent(data);
                    if (kickEvent != null)
                    {
                        await HandleEvent(kickEvent, data);
                    }
                    else
                    {
                        Console.WriteLine("Event not found...\n" + data);
                    }
                    //reset timeout token because just got a new message
                    _timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(PusherConfig.ActivityTimeoutSeconds)).Token;

                    ms.SetLength(0); // Clear the MemoryStream
                    ms.Seek(0, SeekOrigin.Begin);

                }
                catch (ChannelClosedException)
                { }
                catch (TaskCanceledException)
                { }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to produce item " + ex);
                }
            }
            if (_writer.TryComplete())
            {
                Console.WriteLine("Writer closed...");
            }
            else
            {
                Console.WriteLine("Writer already completed!");

            }
        }

        private async Task HandleEvent(KickEvent kickEvent, string data)
        {
            if (kickEvent.Event == KickChatEvents.ChatMessageEvent)
            {
                Console.WriteLine("data");
                await _writer.WriteAsync(ProducerHelper.GetMessageFromString(data));
            }
            else if (kickEvent.Event == KickChatEvents.PongEvent)
            {
                Console.WriteLine("PONG");
            }
            else
            {
                Console.WriteLine("Undefined event: " + kickEvent.Event);
            }
        }
        public static async Task ReccuringSendPing(IKickChatClientWithSend client, TimeSpan timeoutTime, CancellationToken token, CancellationToken timeoutToken)
        {

        }

    }
}

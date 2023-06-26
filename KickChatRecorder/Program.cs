using Cassandra;
using KickChatRecorder.Client;
using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using KickChatRecorder.Service;
using Microsoft.VisualBasic;
using System.Threading.Channels;


///
/*
 * 
 * Implementation using channels seems to be faster wich multiple consumers. 4 provider 6 consumers takes 20s for 10k messages spam from ws, 4 providers 1 consumre takes 60 secodonds. The problem is this solution is I/O bound, meaning when consumers run on multiple threads, it still write to a file by one by one.  
 *  Solution - writing buffer, write to file when X have been filled, in theory where I read this should be faster as there would be less writing operations,
 */
///
// to do now since Im using multiple consumers, it get executed parallel meaning that it will write to file not sequentely if one websocket gives results first etc.
// well I don't really care about that as I can use key:value based db like apache casandra
namespace KickChatRecorder
{
    /// <summary>
    /// add:
    /// when to record - offline/online/always
    /// </summary>
    public class Program
    {
        static async Task Main(string[] args)
        {
            //unbounded rn - would get memory bounded if theres a lot of data coming from the producerss
            var channel = Channel.CreateUnbounded<MessageData>();

            List<Task> tasks = new List<Task>();

            Cluster cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();
            ISession session = cluster.Connect("store");
            CassandraService cassandraService = new CassandraService(session);

            KickChatClientFactory factory = new KickChatClientFactory();

            var channelsToListen = await cassandraService.GetChannels();

            List<Task> socketTasks = new List<Task>();
            List<IKickChatClient> clients = new List<IKickChatClient>();

            foreach (var chan in channelsToListen)
            {
                var client = factory.CreateClient(chan.channel_id);
                var clientConnection = client.ConnectAsync();
                socketTasks.Add(clientConnection);
                clients.Add(client);
            };


            //block the calling thread untill all websockets have established connection
            Task.WaitAll(socketTasks.ToArray());
            //create a producer for each channel listening
            foreach (var client in clients)
            {
                var prod = Task.Run(() =>
                {
                    new Producer(channel.Writer, client);
                });
                tasks.Add(prod);
            }

            //Create 0.5 consumers for each producer
            int consCount = tasks.Count / 2;

            if (consCount < 10)
            {
                consCount = 2;
            }

            for (int i = 0; i < consCount; i++)
            {
                var c = Task.Run(() =>
                {
                    new Consumer(channel.Reader, cassandraService);
                });
                tasks.Add(c);
            }
            //block calling thread - wait, forever pretty much rn
            Task.WaitAll(tasks.ToArray());

            session.Dispose();

            //test
            //for (int i = 0; i < 20; i++)
            //{
            //    var client = factory.CreateTestClient(i.ToString());
            //    await client.ConnectAsync();
            //    var p = Task.Run(() =>
            //    {
            //        new TestProducer(channel.Writer, client);
            //    });
            //    tasks.Add(p);
            //}

            //20prod 20consum - Elapsed Time is 28408 ms
            //20prod 4consum  - Elapsed Time is 37698 ms


        }
    }
}
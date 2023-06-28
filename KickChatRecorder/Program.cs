using Cassandra;
using KickChatRecorder.Client;
using KickChatRecorder.Contracts;
using KickChatRecorder.Helpers;
using KickChatRecorder.Models;
using KickChatRecorder.Service;
using Microsoft.VisualBasic;
using System;
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
    /// 
    /// 1 producer 2 to 2.5 consumers is the fastest per tests
    /// </summary>
    public class Program
    {
        static async Task Main(string[] args)
        {

            var channel = Channel.CreateUnbounded<MessageData>();
            var restartTime = MainHelper.GetTillFutureHour(9);
            var ct = new CancellationTokenSource(restartTime);

            Console.WriteLine("Restart in: " + restartTime.ToString());

            var token = ct.Token;

            List<Task> tasks = new List<Task>();

            Cluster cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();
            ISession session = cluster.Connect("store");
            CassandraService cassandraService = new CassandraService(session);

            var channelsToListen = await cassandraService.GetChannels();

            List<Task> socketTasks = new List<Task>();
            List<IKickChatClientWithSend> clients = new List<IKickChatClientWithSend>();

            KickChatClientFactory factory = new KickChatClientFactory();
            foreach (var chan in channelsToListen)
            {
                var client = factory.CreateClientSendable(chan.channel_id);
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
                    new Producer(channel.Writer, client, token);
                });
                tasks.Add(prod);
            }

            //Create 2 consumers for each producer count
            int consumerCount = tasks.Count * 2;

            Console.WriteLine("Producers: " + tasks.Count);

            for (int i = 0; i < consumerCount; i++)
            {
                var c = Task.Run(() =>
                {
                    new Consumer(channel.Reader, cassandraService);
                });
                tasks.Add(c);
            }

            Console.WriteLine("Consumers: " + consumerCount);
            //block until consumer and producer tasks complete i.e till restart token is canceled
            Task.WaitAll(tasks.ToArray());

            session.Dispose();
        }
    }
}
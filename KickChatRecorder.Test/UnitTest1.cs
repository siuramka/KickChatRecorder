using System.Diagnostics;
using System.Threading.Channels;
using Cassandra;
using KickChatRecorder;
using KickChatRecorder.Client;
using KickChatRecorder.Contracts;
using KickChatRecorder.Models;
using KickChatRecorder.Models.DTO;
using KickChatRecorder.Service;

namespace KickChatRecorder.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Cluster cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();
        ISession session = cluster.Connect("teststore");
        //clear tables
        var clearMessages = session.Execute("TRUNCATE teststore.messages");
        var clearChannels = session.Execute("TRUNCATE teststore.channels");

    }
    /// <summary>
    /// very much not clean code, wrote this just to test performance
    /// </summary>
    [TestCase(20, 60, 2500)]
    [TestCase(20, 50, 2500)]
    [TestCase(20, 40, 2500)]
    [TestCase(20, 20, 2500)]
    public async Task Test1(int producerCount, int consumerCount, int messagesPerProducer)
    {
        var channel = Channel.CreateUnbounded<MessageData>();
        var ct = new CancellationTokenSource(TimeSpan.FromSeconds(99999));
        var token = ct.Token;

        List<Task> producerTasks = new List<Task>();
        List<Task> tasks = new List<Task>();

        Cluster cluster = Cluster.Builder()
        .AddContactPoint("localhost")
        .Build();
        ISession session = cluster.Connect("teststore");
        CassandraService cassandraService = new CassandraService(session);

        KickChatClientFactory factory = new KickChatClientFactory();


        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        List<Task> socketTasks = new List<Task>();
        List<IKickChatClientWithSend> clients = new List<IKickChatClientWithSend>();
        for (int i = 0; i < producerCount; i++)
        {
            //insert channel to db
            await cassandraService.InsertChannel(new Channels()
            { channel_id = i.ToString(), channel_name = i.ToString() });

            var client = factory.CreateTestClientSendable(i.ToString());
            var clientConnection = client.ConnectAsync();
            socketTasks.Add(clientConnection);
            clients.Add(client);
        }

        //block the calling thread untill all websockets have established connection
        Task.WaitAll(socketTasks.ToArray());
        //create a producer for each channel listening
        foreach (var client in clients)
        {
            //send how many messages from params to give back
            await client.Send(messagesPerProducer.ToString());

            var prod = Task.Run(() =>
            {
                new TestProducer(channel.Writer, client, token);
            });
            producerTasks.Add(prod);
        }

        for (int i = 0; i < consumerCount; i++)
        {
            var c = Task.Run(() =>
            {
                new Consumer(channel.Reader, cassandraService);
            });
            tasks.Add(c);
        }
        //block calling thread - wait, forever pretty much rn
        Task.WaitAll(producerTasks.ToArray());
        channel.Writer.TryComplete();

        Task.WaitAll(tasks.ToArray());


        int totalMessages = producerCount * messagesPerProducer;
        var countas = session.Execute("select count(*) from teststore.messages");
        long insertedCount = countas.FirstOrDefault().GetValue<long>("count");

        session.Dispose();
        Assert.AreEqual(insertedCount, producerCount*messagesPerProducer);

    }
}
using Cassandra;
using KickChatRecorder.Client;
using KickChatRecorder.Models;
using KickChatRecorder.Service;
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
            var channel = Channel.CreateUnbounded<MessageData>();
            List<Task> tasks = new List<Task>();

            Cluster cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();

            ISession session = cluster.Connect("store");

            CassandraService cassandraService = new CassandraService(session);

            KickChatClientFactory factory = new KickChatClientFactory();
            var client1 = factory.CreateClient("1202499");
            var client2 = factory.CreateClient("4598");

            var c1c = client1.ConnectAsync();
            var c2c = client2.ConnectAsync();


            Task.WaitAll(c1c, c2c);



            var p1 = Task.Run(() =>
            {
                new Producer(channel.Writer, client1);
            });
            tasks.Add(p1);
            var p2 = Task.Run(() =>
            {
                new Producer(channel.Writer, client2);
            });
            tasks.Add(p2);

            var c1 = Task.Run(() =>
            {
                new Consumer(channel.Reader, cassandraService);
            });
            tasks.Add(c1);

            var c2 = Task.Run(() =>
            {
                new Consumer(channel.Reader, cassandraService);
            });
            tasks.Add(c2);


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

            //for (int i = 0; i < 4; i++)
            //{
            //    var c = Task.Run(() =>
            //    {
            //        new Consumer(channel.Reader, cassandraService);
            //    });
            //    tasks.Add(c);
            //}
            //20prod 20consum - Elapsed Time is 28408 ms
            //20prod 4consum  - Elapsed Time is 37698 ms


            Task.WaitAll(tasks.ToArray());

            session.Dispose();
        }
    }
}
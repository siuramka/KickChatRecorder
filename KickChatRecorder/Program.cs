using Cassandra;
using KickChatRecorder.Client;
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
            var channel = Channel.CreateUnbounded<string>();
            List<Task> tasks = new List<Task>();

            Cluster cluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();

            ISession session = cluster.Connect("store");

            CassandraService cassandraService = new CassandraService(session);

            KickChatClientFactory factory = new KickChatClientFactory();
            //var client1 = factory.CreateTestClient("1");
            //var client2 = factory.CreateTestClient("2");
            //var client3 = factory.CreateTestClient("3");
            //var client4 = factory.CreateTestClient("4");
            //var client5 = factory.CreateTestClient("5");
            //var client6 = factory.CreateTestClient("6");
            //var client7 = factory.CreateTestClient("7");
            //var client8 = factory.CreateTestClient("8");
            //var client9 = factory.CreateTestClient("9");
            //var client10 = factory.CreateTestClient("10");
            ////var client1 = factory.CreateClient("328681");//sam
            ////var client2 = factory.CreateClient("166975");//roshen
            ////var client3 = factory.CreateClient("4598");//action
            ////var client4 = factory.CreateClient("1202499");//hyaba
            ////145222 ice
            ////sam
            ////var client2 = factory.CreateTestClient("2");//roshen
            ////var client3 = factory.CreateTestClient("3");//action
            ////var client4 = factory.CreateTestClient("4");//hyaba

            //var c1c = client1.ConnectAsync();
            //var c2c = client2.ConnectAsync();
            //var c3c = client3.ConnectAsync();
            //var c4c = client4.ConnectAsync();
            //var c5c = client5.ConnectAsync();
            //var c6c = client6.ConnectAsync();
            //var c7c = client7.ConnectAsync();
            //var c8c = client8.ConnectAsync();
            //var c9c = client9.ConnectAsync();
            //var c10c = client10.ConnectAsync();

            //Task.WaitAll(c1c, c2c, c3c,c3c,c5c,c6c,c7c,c8c,c9c,c10c);


            ////var writeToFileStream = Channel.CreateBounded<List<MessageData>>(new BoundedChannelOptions(100)
            ////{
            ////    SingleReader = true,
            ////    SingleWriter = false,
            ////    FullMode = BoundedChannelFullMode.Wait
            ////});
            //var writeToFileStream = Channel.CreateUnbounded<List<MessageData>>();

            //var p1 = Task.Run(() =>
            //{
            //    new Producer(channel.Writer, client1);
            //});
            //tasks.Add(p1);
            //var p2 = Task.Run(() =>
            //{
            //    new Producer(channel.Writer, client2);
            //});
            //tasks.Add(p2);

            for (int i = 0; i < 20; i++)
            {
                var client = factory.CreateTestClient(i.ToString());
                await client.ConnectAsync();
                var p = Task.Run(() =>
                {
                    new TestProducer(channel.Writer, client);
                });
                tasks.Add(p);
            }

            for (int i = 0; i < 4; i++)
            {
                var c = Task.Run(() =>
                {
                    new Consumer(channel.Reader, cassandraService);
                });
                tasks.Add(c);
            }
            //20prod 20consum - Elapsed Time is 28408 ms
            //20prod 4consum  - Elapsed Time is 37698 ms


            Task.WaitAll(tasks.ToArray());

            session.Dispose();
        }
    }
}
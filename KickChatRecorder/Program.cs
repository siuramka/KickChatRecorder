using KickChatRecorder.Models;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;


///
/*
 * 
 * Implementation using channels seems to be faster wich multiple consumers. 4 provider 6 consumers takes 20s for 10k messages spam from ws, 4 providers 1 consumre takes 60 secodonds. The problem is this solution is I/O bound, meaning when consumers run on multiple threads, it still write to a file by one by one.  
 *  Solution - writing buffer, write to file when X have been filled, in theory where I read this should be faster as there would be less writing operations,
 */
///
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
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            KickChatClientFactory factory = new KickChatClientFactory();
            var client1 = factory.CreateClient("145222");//sam
            var client2 = factory.CreateClient("4598");//sam

            //var client1 = factory.CreateClient("328681");//sam
            //var client2 = factory.CreateClient("166975");//roshen
            //var client3 = factory.CreateClient("4598");//action
            //var client4 = factory.CreateClient("1202499");//hyaba
            //145222 ice
            ;//sam
            //var client2 = factory.CreateTestClient("2");//roshen
            //var client3 = factory.CreateTestClient("3");//action
            //var client4 = factory.CreateTestClient("4");//hyaba

            var c1c = client1.ConnectAsync();
            var c2c = client2.ConnectAsync();

            Task.WaitAll(c1c, c2c);

            var channel = Channel.CreateUnbounded<string>();
            //var writeToFileStream = Channel.CreateBounded<List<MessageData>>(new BoundedChannelOptions(100)
            //{
            //    SingleReader = true,
            //    SingleWriter = false,
            //    FullMode = BoundedChannelFullMode.Wait
            //});
            var writeToFileStream = Channel.CreateUnbounded<List<MessageData>>();
            List<Task> tasks = new List<Task>();
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

            //for (int i = 0; i < 4; i++)
            //{
            //    var client = factory.CreateTestClient(i.ToString());
            //    await client.ConnectAsync();
            //    var p = Task.Run(() =>
            //    {
            //        new Producer(channel.Writer, client);
            //    });
            //    tasks.Add(p);
            //}

            for (int i = 0; i < 1; i++)
            {
                var c = Task.Run(() =>
                {
                    new Consumer(channel.Reader, writeToFileStream.Writer);
                });
                tasks.Add(c);
            }

            var cstream = Task.Run(() =>
            {
                new FileStreamConsumer(writeToFileStream.Reader);
            });
            tasks.Add(cstream);


            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();

            Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
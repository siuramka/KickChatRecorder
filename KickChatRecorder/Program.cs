using KickChatRecorder.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;

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
            KickChatClientFactory factory = new KickChatClientFactory();
            var client1 = factory.CreateClient("328681");//sam
            var client2 = factory.CreateClient("166975");//roshen
            var client3 = factory.CreateClient("4598");//action
            var client4 = factory.CreateClient("1202499");//hyaba


            var c1c = client1.ConnectAsync();
            var c2c = client2.ConnectAsync();
            var c3c = client3.ConnectAsync();
            var c4c = client4.ConnectAsync();

            Task.WaitAll(c1c, c2c,c3c,c4c);

            var channel = Channel.CreateUnbounded<string>();


            var p = Task.Run(() =>
            {
                new Producer(channel.Writer, client1);
            });
            var p2 = Task.Run(() =>
            {
                new Producer(channel.Writer, client2);
            });
            var p3 = Task.Run(() =>
            {
                new Producer(channel.Writer, client3);
            });
            var p4 = Task.Run(() =>
            {
                new Producer(channel.Writer, client4);
            });


            var c1 = Task.Run(() =>
            {
                new Consumer(channel.Reader);
            });
            var c2 = Task.Run(() =>
            {
                new Consumer(channel.Reader);
            });

            Task.WaitAll(p,p2,p3,p4,c1, c2);
        }
    }
}
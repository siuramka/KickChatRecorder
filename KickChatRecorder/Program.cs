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

            KickChatClientConfiguration configas = new KickChatClientConfiguration("7022952");
            

            KickChatClient client1 = new KickChatClient(configas);
            await client1.ConnectAsync();

            KickChatClientConfiguration configas2 = new KickChatClientConfiguration("4598");

            KickChatClient client2 = new KickChatClient(configas2);
            await client2.ConnectAsync();



            var channel = Channel.CreateUnbounded<string>();


            var p = Task.Run(() =>
            {
                new Producer(channel.Writer, client1);
            });
            var p2 = Task.Run(() =>
            {
                new Producer(channel.Writer, client2);
            });

            var c1 = Task.Run(() =>
            {
                new Consumer(channel.Reader);
            });

            Task.WaitAll(p,p2,c1);
        }
    }
}
using KickChatRecorder.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace KickChatRecorder
{

    public class Program
    {
        static async Task Main(string[] args)
        {
            KickChatClientConfiguration configas = new KickChatClientConfiguration();
            configas.ChatroomId = "7022952";
            KickChatClient client1 = new KickChatClient(configas);
            await client1.ConnectAsync();

            Console.ReadKey();
        }
    }
}
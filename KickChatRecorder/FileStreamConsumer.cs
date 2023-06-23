using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KickChatRecorder
{
    public class FileStreamConsumer
    {
        private static readonly object _fileLock = new object();
        private ChannelReader<List<MessageData>> _reader;
        public FileStreamConsumer(ChannelReader<List<MessageData>> reader)
        {
            _reader = reader;
            Task.WaitAll(this.Run());
        }
        public async Task Run()
        {
            while (await _reader.WaitToReadAsync())
            {

                var item = await _reader.ReadAsync();

                try
                {
                    List<MessageData> messages = new List<MessageData>(item);
                    var messagesByFile = messages.GroupBy(m => m.Channel);
                    foreach(var message in messagesByFile)
                    {
                        string textWall = "";
                        foreach(var messageData in message)
                        {
                            textWall += messageData.ToString() + "\n";
                        }
                        lock(_fileLock)
                        {
                            File.AppendAllText($"{message.Key}.txt", textWall);
                        }
                    }
                    //Console.WriteLine(messageData.ToString() + "\n");
                    //File.AppendAllText(fileName, messageData.ToString() + "\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FileStream channel:" + ex);
                    Console.WriteLine(item);
                }

            }
        }
    }
}

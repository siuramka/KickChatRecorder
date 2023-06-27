using KickChatRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KickChatRecorder.Helpers
{
    public static class PusherHelper
    {
        public static KickEvent? GetPusherEvent(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<KickEvent>(json);

            }
            catch
            {
                return null;
            }
        }
    }
}

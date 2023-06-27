using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Models.Config
{
    public static class PusherConfig
    {
        public static readonly string SecureConenctionString = "wss://ws-us2.pusher.com/app/eb1d5f283081a78b932c?protocol=7&client=js&version=7.4.0&flash=false";

        // If pusher doesnt send any messages it sets a timer for a random time >= 2 minutes <= 30 minutes(I think), and if it doesnt receive a ping message it drops the connection
        // at what perdiod to send ping if client hasn't received a message.
        public static int ActivityTimeoutSeconds = 5;
    }
}

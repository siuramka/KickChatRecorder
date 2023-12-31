﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Models.Config
{
    public static class KickChatEvents
    {
        //User writes a message event.
        public static string ChatMessageEvent = @"App\Events\ChatMessageEvent";
        public static string PongEvent = @"pusher:pong";
        public static string Connected = @"pusher:connection_established";
        public static string Subscribed = @"pusher_internal:subscription_succeeded";

    }
}

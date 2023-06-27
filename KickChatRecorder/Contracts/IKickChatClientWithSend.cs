using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Contracts
{
    public interface IKickChatClientWithSend : IKickChatClient
    {
        Task Send(string data);
    }
}

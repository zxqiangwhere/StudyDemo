using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.AgentComponents
{
    public interface ITcpAgent:IAgent
    {
        bool IsReuseAddress { get; set; }
        uint AcceptSocketCount { get; set; }
        uint SocketBufferSize { get; set; }
        uint SocketListenQueue { get; set; }
        uint KeepAliveTime { get; set; }
        uint KeepAliveInterval { get; set; }

        bool SendSmallFile(IntPtr connId, string lpszFileName, ref WSABUF pHead, ref WSABUF pTail);
    }
}

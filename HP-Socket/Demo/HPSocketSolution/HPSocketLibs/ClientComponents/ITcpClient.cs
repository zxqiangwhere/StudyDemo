using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public interface ITcpClient : IClient
    {
        uint SocketBufferSize { get; set; }
        uint KeepAliveTime { get; set; }
        uint KeepAliveInterval { get; set; }

        bool SendSmallFile(string lpszFileName, ref WSABUF pHead, ref WSABUF pTail);
    }
}

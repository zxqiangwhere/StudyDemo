using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public interface IClient
    {
        bool IsSecure { get; }
        bool IsStarted { get; }
        ServiceState State { get; }
        SocketError ErrorCode { get; }
        string ErrorMessage { get; }
        IntPtr ConnectionId { get; }

        IntPtr ExtraData { get; set; }
        uint FreeBufferPoolSize { get; set; }
        uint FreeBufferPoolHold { get; set; }

        bool Start(string pszRemoteAddress, ushort usPort, bool bAsyncConnect);
        bool StartWithBindAddress(string lpszRemoteAddress, ushort usPort, bool bAsyncConnect, string lpszBindAddress);
        bool Stop();
        void Destroy();
        bool Send(byte[] pBuffer, int length);
        bool Send(IntPtr pBuffer, int length);
        bool SendPart(byte[] pBuffer, int length, int iOffset);
        bool SendPart(IntPtr pBuffer, int length, int iOffset);
        bool SendPackets(WSABUF[] pBuffers, int iCount);
        bool GetLocalAddress(ref string ip, ref ushort port);
        bool GetRemoteHost(ref string ip, ref ushort port);
        bool GetPendingDataLength(ref int length);
    }
}

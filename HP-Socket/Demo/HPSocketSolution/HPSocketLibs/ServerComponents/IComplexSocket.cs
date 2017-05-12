using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public interface IComplexSocket
    {
        bool IsSecure { get;  }
        bool IsStarted { get; }
        ServiceState State { get; }
        uint MaxConnectionCount { get; set; }
        uint ConnectionCount { get; }
        SocketError ErrorCode { get; }
        string ErrorMessage { get; }
        SendPolicy SendPolicy { get; set; }
        bool MarkSilence { get; set; }
        uint FreeSocketObjLockTime { get; set; }
        uint FreeSocketObjPool { get; set; }
        uint FreeBufferObjPool { get; set; }
        uint FreeSocketObjHold { get; set; }
        uint FreeBufferObjHold { get; set; }
        uint WorkerThreadCount { get; set; }

        
        bool Stop();
        void Destroy();
        bool Send( IntPtr connId, byte[] pBuffer, int length);
        bool Send(IntPtr connId, IntPtr pBuffer, int length);
        bool SendPart( IntPtr connId, byte[] pBuffer, int length, int iOffset);
        bool SendPart(IntPtr connId, IntPtr pBuffer, int length, int iOffset);
        bool SendPackets(IntPtr connId, WSABUF[] pBuffers, int iCount);
        bool Disconnect( IntPtr connId, bool bForce);
        bool DisconnectLongConnections( uint dwPeriod, bool bForce);
        bool DisconnectSilenceConnections( uint dwPeriod, bool bForce);
        IntPtr[] GetAllConnectionIDs();
        bool GetConnectPeriod(IntPtr connId, ref uint period);
        bool GetSilencePeriod(IntPtr connId, ref uint period);
        bool GetLocalAddress(IntPtr connId, ref string ip, ref ushort port);
        bool GetRemoteAddress(IntPtr connId, ref string ip, ref ushort port);
        bool GetPendingDataLength(IntPtr connId, ref int length);
        bool GetConnectionExtra(IntPtr connId, ref IntPtr ConnectionExtra);
        bool SetConnectionExtra(IntPtr connId, IntPtr ConnectionExtra);

    }
}

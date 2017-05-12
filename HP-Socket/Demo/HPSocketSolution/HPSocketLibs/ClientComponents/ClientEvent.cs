using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public class ClientEvent
    {
        public delegate HandleResult OnTcpPrepareConnectEventHandler(TcpClient sender, IntPtr socket);
        public delegate HandleResult OnTcpConnectEventHandler(TcpClient sender);
        public delegate HandleResult OnTcpSendEventHandler(TcpClient sender, byte[] bytes);
        public delegate HandleResult OnTcpReceiveEventHandler(TcpClient sender, byte[] bytes);
        //Tcp Pull
        public delegate HandleResult OnTcpPullReceiveEventHandler(TcpClient sender, int length);
        public delegate HandleResult OnTcpPointerDataReceiveEventHandler(TcpClient sender, IntPtr pData, int length);
        public delegate HandleResult OnTcpCloseEventHandler(TcpClient sender, SocketOperation enOperation, int errorCode);
        public delegate HandleResult OnTcpHandShakeEventHandler(TcpClient sender);

        //UDP
        public delegate HandleResult OnUdpPrepareConnectEventHandler(UdpClient sender, IntPtr socket);
        public delegate HandleResult OnUdpConnectEventHandler(UdpClient sender);
        public delegate HandleResult OnUdpSendEventHandler(UdpClient sender, byte[] bytes);
        public delegate HandleResult OnUdpReceiveEventHandler(UdpClient sender, byte[] bytes);
        public delegate HandleResult OnUdpPointerDataReceiveEventHandler(UdpClient sender, IntPtr pData, int length);
        public delegate HandleResult OnUdpCloseEventHandler(UdpClient sender, SocketOperation enOperation, int errorCode);
        public delegate HandleResult OnUdpHandShakeEventHandler(UdpClient sender);
    }
}

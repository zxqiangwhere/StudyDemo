using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public class UdpClient : IUdpClient
    {
        public uint DetectAttempts
        {
            get
            {
                return SdkFunctions.HP_UdpClient_GetDetectAttempts(pClient);
            }

            set
            {
                SdkFunctions.HP_UdpClient_SetDetectAttempts(pClient, value);
            }
        }

        public uint DetectInterval
        {
            get
            {
                return SdkFunctions.HP_UdpClient_GetDetectInterval(pClient);
            }

            set
            {
                SdkFunctions.HP_UdpClient_SetDetectInterval(pClient, value);
            }
        }

        public uint MaxDatagramSize
        {
            get
            {
                return SdkFunctions.HP_UdpClient_GetMaxDatagramSize(pClient);
            }

            set
            {
                SdkFunctions.HP_UdpClient_SetMaxDatagramSize(pClient, value);
            }
        }
        public IntPtr ConnectionId
        {
            get
            {
                return SdkFunctions.HP_Client_GetConnectionID(pClient);
            }
        }

        public SocketError ErrorCode
        {
            get
            {
                return SdkFunctions.HP_Client_GetLastError(pClient);
            }
        }

        public string ErrorMessage
        {
            get
            {
                IntPtr ptr = SdkFunctions.HP_Client_GetLastErrorDesc(pClient);
                string desc = System.Runtime.InteropServices.Marshal.PtrToStringUni(ptr);
                return desc;
            }
        }

        public IntPtr ExtraData
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public uint FreeBufferPoolHold
        {
            get
            {
                return SdkFunctions.HP_Client_GetFreeBufferPoolHold(pClient);
            }

            set
            {
                SdkFunctions.HP_Client_SetFreeBufferPoolHold(pClient, value);
            }
        }

        public uint FreeBufferPoolSize
        {
            get
            {
                return SdkFunctions.HP_Client_GetFreeBufferPoolSize(pClient);
            }

            set
            {
                SdkFunctions.HP_Client_SetFreeBufferPoolSize(pClient, value);
            }
        }

        public bool IsSecure
        {
            get
            {
                return SdkFunctions.HP_Client_IsSecure(pClient);
            }
        }

        public bool IsStarted
        {
            get
            {
                return SdkFunctions.HP_Client_HasStarted(pClient);
            }
        }

        public ServiceState State
        {
            get
            {
                return SdkFunctions.HP_Client_GetState(pClient);
            }
        }

        protected IntPtr _pClient = IntPtr.Zero;
        protected IntPtr pClient
        {
            get
            {
                return _pClient;
            }

            set
            {
                _pClient = value;
            }
        }

        protected IntPtr pListener = IntPtr.Zero;

        protected bool IsCreate = false;
        //ConnectionExtra ExtraData = new ConnectionExtra();
        protected SdkFunctions.OnPrepareConnect _OnPrepareConnect = null;
        protected SdkFunctions.OnConnect _OnConnect = null;
        protected SdkFunctions.OnReceive _OnReceive = null;
        protected SdkFunctions.OnSend _OnSend = null;
        protected SdkFunctions.OnClose _OnClose = null;
        protected SdkFunctions.OnHandShake _OnHandShake = null;
        /// <summary>
        /// 准备连接了事件
        /// </summary>
        public event ClientEvent.OnUdpPrepareConnectEventHandler OnPrepareConnect;
        /// <summary>
        /// 连接事件
        /// </summary>
        public event ClientEvent.OnUdpConnectEventHandler OnConnect;
        /// <summary>
        /// 数据发送事件
        /// </summary>
        public event ClientEvent.OnUdpSendEventHandler OnSend;
        /// <summary>
        /// 数据到达事件
        /// </summary>
        public event ClientEvent.OnUdpReceiveEventHandler OnReceive;
        /// <summary>
        /// 数据到达事件(指针数据)
        /// </summary>
        public event ClientEvent.OnUdpPointerDataReceiveEventHandler OnPointerDataReceive;
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        public event ClientEvent.OnUdpCloseEventHandler OnClose;
        /// <summary>
        /// 握手事件
        /// </summary>
        public event ClientEvent.OnUdpHandShakeEventHandler OnHandShake;

        public UdpClient()
        {
            CreateListener();
        }
        /// <summary>
        /// 停止通讯组件
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (IsStarted == false)
            {
                return false;
            }
            return SdkFunctions.HP_Client_Stop(pClient);
        }

        public virtual void Destroy()
        {
            Stop();

            if (pClient != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_UdpClient(pClient);
                pClient = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_UdpClientListener(pListener);
                pListener = IntPtr.Zero;
            }

            IsCreate = false;
        }
        public virtual bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pClient != IntPtr.Zero)
            {
                return false;
            }

            pListener = SdkFunctions.Create_HP_UdpClientListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pClient = SdkFunctions.Create_HP_UdpClient(pListener);
            if (pClient == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            return true;
        }

        protected virtual void SetCallback()
        {
            _OnPrepareConnect = new SdkFunctions.OnPrepareConnect(SDK_OnPrepareConnect);
            _OnConnect = new SdkFunctions.OnConnect(SDK_OnConnect);
            _OnSend = new SdkFunctions.OnSend(SDK_OnSend);
            _OnReceive = new SdkFunctions.OnReceive(SDK_OnReceive);
            _OnClose = new SdkFunctions.OnClose(SDK_OnClose);
            _OnHandShake = new SdkFunctions.OnHandShake(SDK_OnHandShake);
        }

        private HandleResult SDK_OnHandShake(IntPtr pSender, IntPtr connId)
        {
            if (OnHandShake != null)
            {
                return OnHandShake(this);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnClose(IntPtr pSender, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            if (OnClose != null)
            {
                return OnClose(this, enOperation, errorCode);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnReceive(IntPtr pSender, IntPtr connId, IntPtr pData, int length)
        {
            if (OnPointerDataReceive != null)
            {
                return OnPointerDataReceive(this, pData, length);
            }
            else if (OnReceive != null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnReceive(this, bytes);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnSend(IntPtr pSender, IntPtr connId, IntPtr pData, int length)
        {
            if (OnSend != null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnSend(this, bytes);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnConnect(IntPtr pSender, IntPtr connId)
        {
            if (OnConnect != null)
            {
                return OnConnect(this);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnPrepareConnect(IntPtr pSender, IntPtr connId, IntPtr socket)
        {
            if (OnPrepareConnect != null)
            {
                return OnPrepareConnect(this, socket);
            }
            return HandleResult.Ignore;
        }

        public bool GetLocalAddress(ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = SdkFunctions.HP_Client_GetLocalAddress(pClient, sb, ref ipLength, ref port);
            if (ret == true)
            {
                ip = sb.ToString();
            }
            return ret;
        }

        public bool GetPendingDataLength(ref int length)
        {
            return SdkFunctions.HP_Client_GetPendingDataLength(pClient, ref length);
        }

        public bool GetRemoteHost(ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = SdkFunctions.HP_Client_GetRemoteHost(pClient, sb, ref ipLength, ref port);
            if (ret == true)
            {
                ip = sb.ToString();
            }
            return ret;
        }

        public bool Send(IntPtr pBuffer, int length)
        {
            return SdkFunctions.HP_Client_Send(pClient, pBuffer, length);
        }

        public bool Send(byte[] pBuffer, int length)
        {
            return SdkFunctions.HP_Client_Send(pClient, pBuffer, length);
        }
        public bool Send<T>(T obj)
        {
            byte[] buffer = ConverterHelper.StructureToByte<T>(obj);
            return Send(buffer, buffer.Length);
        }
        public bool SendBySerializable(object obj)
        {
            byte[] buffer = ConverterHelper.ObjectToBytes(obj);
            return Send(buffer, buffer.Length);
        }
        public bool SendPackets(WSABUF[] pBuffers, int iCount)
        {
            return SdkFunctions.HP_Client_SendPackets(pClient, pBuffers, iCount);
        }
        public bool SendPackets<T>(T[] objects)
        {
            bool ret = false;

            WSABUF[] buffer = new WSABUF[objects.Length];
            IntPtr[] ptrs = new IntPtr[buffer.Length];
            try
            {

                for (int i = 0; i < objects.Length; i++)
                {
                    buffer[i].Length = Marshal.SizeOf(typeof(T));

                    ptrs[i] = Marshal.AllocHGlobal(buffer[i].Length);
                    Marshal.StructureToPtr(objects[i], ptrs[i], true);

                    buffer[i].Buffer = ptrs[i];
                }
                ret = SendPackets(buffer, buffer.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                for (int i = 0; i < ptrs.Length; i++)
                {
                    if (ptrs[i] != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(ptrs[i]);
                    }
                }
            }

            return ret;
        }
        public bool SendPart(IntPtr pBuffer, int length, int iOffset)
        {
            return SdkFunctions.HP_Client_SendPart(pClient, pBuffer, length, iOffset);
        }

        public bool SendPart(byte[] pBuffer, int length, int iOffset)
        {
            return SdkFunctions.HP_Client_SendPart(pClient, pBuffer, length, iOffset);
        }

        public bool Start(string pszRemoteAddress, ushort usPort, bool bAsyncConnect)
        {
            if (string.IsNullOrEmpty(pszRemoteAddress) == true)
            {
                throw new Exception("address is null");
            }
            else if (usPort == 0)
            {
                throw new Exception("port is zero");
            }

            if (IsStarted == true)
            {
                return false;
            }

            this.SetCallback();

            return SdkFunctions.HP_Client_Start(pClient, pszRemoteAddress, usPort, bAsyncConnect);
        }

        public bool StartWithBindAddress(string lpszRemoteAddress, ushort usPort, bool bAsyncConnect, string lpszBindAddress)
        {
            if (string.IsNullOrEmpty(lpszRemoteAddress) == true)
            {
                throw new Exception("address is null");
            }
            else if (usPort == 0)
            {
                throw new Exception("port is zero");
            }

            if (IsStarted == true)
            {
                return false;
            }

            this.SetCallback();

            return SdkFunctions.HP_Client_StartWithBindAddress(pClient, lpszRemoteAddress, usPort, bAsyncConnect, lpszBindAddress);
        }
    }
}

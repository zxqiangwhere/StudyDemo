using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public class UdpServer : IUdpServer
    {

        public uint ConnectionCount
        {
            get
            {
                return SdkFunctions.HP_Server_GetConnectionCount(pServer);
            }
        }
        public SocketError ErrorCode
        {
            get
            {
                return SdkFunctions.HP_Server_GetLastError(pServer);
            }
        }

        public string ErrorMessage
        {
            get
            {
                IntPtr ptr = SdkFunctions.HP_Server_GetLastErrorDesc(pServer);
                string desc = Marshal.PtrToStringUni(ptr);
                return desc;
            }
        }

        public uint FreeBufferObjHold
        {
            get
            {
                return SdkFunctions.HP_Server_GetFreeBufferObjHold(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetFreeBufferObjHold(pServer, value);
            }
        }

        public uint FreeBufferObjPool
        {

            get
            {
                return SdkFunctions.HP_Server_GetFreeBufferObjPool(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetFreeBufferObjPool(pServer, value);
            }
        }

        public uint FreeSocketObjHold
        {
            get
            {
                return SdkFunctions.HP_Server_GetFreeSocketObjHold(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetFreeSocketObjHold(pServer, value);
            }
        }

        public uint FreeSocketObjLockTime
        {
            get
            {
                return SdkFunctions.HP_Server_GetFreeSocketObjLockTime(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetFreeSocketObjLockTime(pServer, value);
            }
        }

        public uint FreeSocketObjPool
        {
            get
            {
                return SdkFunctions.HP_Server_GetFreeSocketObjPool(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetFreeSocketObjPool(pServer, value);
            }
        }

        public bool IsSecure
        {

            get
            {
                return SdkFunctions.HP_Server_IsSecure(pServer);
            }
        }

        public bool IsStarted
        {
            get
            {
                if (pServer == IntPtr.Zero)
                {
                    return false;
                }
                return SdkFunctions.HP_Server_HasStarted(pServer);
            }
        }
        /// <summary>
        /// 读取或设置是否标记静默时间（设置为 TRUE 时 DisconnectSilenceConnections() 和 GetSilencePeriod() 才有效，默认：FALSE）
        /// </summary>
        public bool MarkSilence
        {
            get
            {
                return SdkFunctions.HP_Server_IsMarkSilence(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetMarkSilence(pServer, value);
            }
        }

        public uint MaxConnectionCount
        {
            get
            {
                return SdkFunctions.HP_Server_GetMaxConnectionCount(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetMaxConnectionCount(pServer, value);
            }
        }

        public SendPolicy SendPolicy
        {
            get
            {
                return SdkFunctions.HP_Server_GetSendPolicy(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetSendPolicy(pServer, value);
            }
        }

        public ServiceState State
        {
            get
            {
                return SdkFunctions.HP_Server_GetState(pServer);
            }
        }

        public uint WorkerThreadCount
        {
            get
            {
                return SdkFunctions.HP_Server_GetWorkerThreadCount(pServer);
            }
            set
            {
                SdkFunctions.HP_Server_SetWorkerThreadCount(pServer, value);
            }
        }

        private IntPtr _pServer = IntPtr.Zero;
        protected IntPtr pServer
        {
            get
            {
                return _pServer;
            }

            set
            {
                _pServer = value;
            }
        }
        protected IntPtr pListener = IntPtr.Zero;
        protected bool IsCreate = false;
        /// <summary>
        /// 服务器ip
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort Port { get; set; }

        public uint MaxDatagramSize
        {
            get
            {
                return SdkFunctions.HP_UdpServer_GetMaxDatagramSize(pServer);
            }

            set
            {
                SdkFunctions.HP_UdpServer_SetMaxDatagramSize(pServer, value);
            }
        }

        public uint DetectAttempts
        {
            get
            {
                return SdkFunctions.HP_UdpServer_GetDetectAttempts(pServer);
            }

            set
            {
                SdkFunctions.HP_UdpServer_SetDetectAttempts(pServer, value);
            }
        }

        public uint DetectInterval
        {
            get
            {
                return SdkFunctions.HP_UdpServer_GetDetectInterval(pServer);
            }

            set
            {
                SdkFunctions.HP_UdpServer_SetDetectInterval(pServer, value);
            }
        }

        public uint PostReceiveCount
        {
            get
            {
                return SdkFunctions.HP_UdpServer_GetPostReceiveCount(pServer);
            }

            set
            {
                SdkFunctions.HP_UdpServer_SetPostReceiveCount(pServer, value);
            }
        }

        protected SdkFunctions.OnPrepareListen _OnPrepareListen = null;
        protected SdkFunctions.OnAccept _OnAccept = null;
        protected SdkFunctions.OnReceive _OnReceive = null;
        protected SdkFunctions.OnSend _OnSend = null;
        protected SdkFunctions.OnClose _OnClose = null;
        protected SdkFunctions.OnShutdown _OnShutdown = null;
        protected SdkFunctions.OnHandShake _OnHandShake = null;

        /// <summary>
        /// 连接到达事件
        /// </summary>
        public event ServerEvent.OnAcceptEventHandler OnAccept;
        /// <summary>
        /// 数据包发送事件
        /// </summary>
        public event ServerEvent.OnSendEventHandler OnSend;
        /// <summary>
        /// 准备监听了事件
        /// </summary>
        public event ServerEvent.OnPrepareListenEventHandler OnPrepareListen;
        /// <summary>
        /// 数据到达事件
        /// </summary>
        public event ServerEvent.OnReceiveEventHandler OnReceive;
        /// <summary>
        /// 数据到达事件(指针数据)
        /// </summary>
        public event ServerEvent.OnPointerDataReceiveEventHandler OnPointerDataReceive;
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        public event ServerEvent.OnCloseEventHandler OnClose;
        /// <summary>
        /// 服务器关闭事件
        /// </summary>
        public event ServerEvent.OnShutdownEventHandler OnShutdown;
        /// <summary>
        /// 握手成功事件
        /// </summary>
        public event ServerEvent.OnHandShakeEventHandler OnHandShake;

        public UdpServer()
        {
            CreateListener();
        }
        ~UdpServer()
        {
            Destroy();
        }
        protected virtual bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pServer != IntPtr.Zero)
            {
                return false;
            }

            pListener = SdkFunctions.Create_HP_UdpServerListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }
            pServer = SdkFunctions.Create_HP_UdpServer(pListener);
            if (pServer == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            return true;
        }

        public virtual void Destroy()
        {
            Stop();
            if (pServer != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_UdpServer(pServer);
                pServer = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_UdpServerListener(pListener);
                pListener = IntPtr.Zero;
            }

            IsCreate = false;
        }

        public bool Disconnect(IntPtr connId, bool bForce = true)
        {
            return SdkFunctions.HP_Server_Disconnect(pServer, connId, bForce);
        }

        public bool DisconnectLongConnections(uint dwPeriod, bool bForce)
        {
            return SdkFunctions.HP_Server_DisconnectLongConnections(pServer, dwPeriod, bForce);
        }

        public bool DisconnectSilenceConnections(uint dwPeriod, bool bForce)
        {
            return SdkFunctions.HP_Server_DisconnectSilenceConnections(pServer, dwPeriod, bForce);
        }

        public IntPtr[] GetAllConnectionIDs()
        {
            IntPtr[] arr = null;
            do
            {
                uint count = ConnectionCount;
                if (count == 0)
                {
                    break;
                }
                arr = new IntPtr[count];
                if (SdkFunctions.HP_Server_GetAllConnectionIDs(pServer, arr, ref count))
                {
                    if (arr.Length > count)
                    {
                        IntPtr[] newArr = new IntPtr[count];
                        Array.Copy(arr, newArr, count);
                        arr = newArr;
                    }
                    break;
                }
            } while (true);

            return arr;
        }

        public bool GetConnectPeriod(IntPtr connId, ref uint period)
        {
            return SdkFunctions.HP_Server_GetConnectPeriod(pServer, connId, ref period);
        }

        public bool GetListenAddress(ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = SdkFunctions.HP_Server_GetListenAddress(pServer, sb, ref ipLength, ref port);
            if (ret == true)
            {
                ip = sb.ToString();
            }
            return ret;
        }

        public bool GetLocalAddress(IntPtr connId, ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = SdkFunctions.HP_Server_GetLocalAddress(pServer, connId, sb, ref ipLength, ref port) && ipLength > 0;
            if (ret == true)
            {
                ip = sb.ToString();
            }

            return ret;
        }

        public bool GetPendingDataLength(IntPtr connId, ref int length)
        {
            return SdkFunctions.HP_Server_GetPendingDataLength(pServer, connId, ref length);
        }

        public bool GetRemoteAddress(IntPtr connId, ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = SdkFunctions.HP_Server_GetRemoteAddress(pServer, connId, sb, ref ipLength, ref port) && ipLength > 0;
            if (ret == true)
            {
                ip = sb.ToString();
            }

            return ret;
        }

        public bool GetSilencePeriod(IntPtr connId, ref uint period)
        {
            return SdkFunctions.HP_Server_GetSilencePeriod(pServer, connId, ref period);
        }

        public bool Send(IntPtr connId, IntPtr pBuffer, int length)
        {
            return SdkFunctions.HP_Server_Send(pServer, connId, pBuffer, length);
        }

        public bool Send(IntPtr connId, byte[] pBuffer, int length)
        {
            return SdkFunctions.HP_Server_Send(pServer, connId, pBuffer, length);
        }
        public bool Send<T>(IntPtr connId, T obj)
        {
            byte[] buffer = ConverterHelper.StructureToByte<T>(obj);
            return Send(connId, buffer, buffer.Length);
        }

        public bool SendBySerializable(IntPtr connId, object obj)
        {
            byte[] buffer = ConverterHelper.ObjectToBytes(obj);
            return Send(connId, buffer, buffer.Length);
        }

        public bool SendPackets(IntPtr connId, WSABUF[] pBuffers, int iCount)
        {
            return SdkFunctions.HP_Server_SendPackets(pServer, connId, pBuffers, iCount);
        }
        public bool SendPackets<T>(IntPtr connId, T[] objects)
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
                ret = SendPackets(connId, buffer, buffer.Length);
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

        public bool SendPart(IntPtr connId, IntPtr pBuffer, int length, int iOffset)
        {
            return SdkFunctions.HP_Server_SendPart(pServer, connId, pBuffer, length, iOffset);
        }

        public bool SendPart(IntPtr connId, byte[] pBuffer, int length, int iOffset)
        {
            return SdkFunctions.HP_Server_SendPart(pServer, connId, pBuffer, length, iOffset);
        }

        public bool Start(string pszBindAddress, ushort usPort)
        {
            if (IsCreate == false)
            {
                return false;
            }
            if (IsStarted == true)
            {
                return false;
            }

            SetCallback();

            return SdkFunctions.HP_Server_Start(pServer, IpAddress, Port);
        }

        public bool Stop()
        {
            if (IsStarted == false)
            {
                return false;
            }

            return SdkFunctions.HP_Server_Stop(pServer);
        }

        protected virtual void SetCallback()
        {
            _OnPrepareListen = new SdkFunctions.OnPrepareListen(SDK_OnPrepareListen);
            _OnAccept = new SdkFunctions.OnAccept(SDK_OnAccept);
            _OnSend = new SdkFunctions.OnSend(SDK_OnSend);
            _OnReceive = new SdkFunctions.OnReceive(SDK_OnReceive);
            _OnClose = new SdkFunctions.OnClose(SDK_OnClose);
            _OnShutdown = new SdkFunctions.OnShutdown(SDK_OnShutdown);
            _OnHandShake = new SdkFunctions.OnHandShake(SDK_OnHandShake);
        }
        protected HandleResult SDK_OnHandShake(IntPtr pSender, IntPtr connId)
        {
            if (OnHandShake != null)
            {
                return OnHandShake(connId);
            }
            return HandleResult.Ignore;
        }

        protected HandleResult SDK_OnPrepareListen(IntPtr pSender, IntPtr soListen)
        {
            if (OnPrepareListen != null)
            {
                return OnPrepareListen(soListen);
            }
            return HandleResult.Ignore;
        }

        protected HandleResult SDK_OnAccept(IntPtr pSender, IntPtr connId, IntPtr pClient)
        {
            if (OnAccept != null)
            {
                return OnAccept(connId, pClient);
            }

            return HandleResult.Ignore;
        }

        protected HandleResult SDK_OnSend(IntPtr pSender, IntPtr connId, IntPtr pData, int length)
        {
            if (OnSend != null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnSend(connId, bytes);
            }
            return HandleResult.Ignore;
        }

        protected HandleResult SDK_OnReceive(IntPtr pSender, IntPtr connId, IntPtr pData, int length)
        {
            if (OnPointerDataReceive != null)
            {
                return OnPointerDataReceive(connId, pData, length);
            }
            else if (OnReceive != null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnReceive(connId, bytes);
            }
            return HandleResult.Ignore;
        }

        protected HandleResult SDK_OnClose(IntPtr pSender, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            if (OnClose != null)
            {
                return OnClose(connId, enOperation, errorCode);
            }
            return HandleResult.Ignore;
        }

        protected HandleResult SDK_OnShutdown(IntPtr pSender)
        {
            if (OnShutdown != null)
            {
                return OnShutdown();
            }
            return HandleResult.Ignore;
        }

        public bool GetConnectionExtra(IntPtr connId, ref IntPtr ConnectionExtra)
        {
            return SdkFunctions.HP_Server_GetConnectionExtra(pServer, connId, ref ConnectionExtra);
        }

        public bool SetConnectionExtra(IntPtr connId, IntPtr ConnectionExtra)
        {
            return SdkFunctions.HP_Server_SetConnectionExtra(pServer, connId, ConnectionExtra);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace HPSocketCS
{
    public class UdpServerEvent
    {
        public delegate HandleResult OnSendEventHandler(IntPtr connId, byte[] bytes);
        public delegate HandleResult OnReceiveEventHandler(IntPtr connId, byte[] bytes);
        public delegate HandleResult OnPointerDataReceiveEventHandler(IntPtr connId, IntPtr pData, int length);
        public delegate HandleResult OnCloseEventHandler(IntPtr connId, SocketOperation enOperation, int errorCode);
        public delegate HandleResult OnShutdownEventHandler();
        public delegate HandleResult OnPrepareListenEventHandler(IntPtr soListen);
        public delegate HandleResult OnAcceptEventHandler(IntPtr connId, IntPtr pClient);
        public delegate HandleResult OnHandShakeEventHandler(IntPtr connId);
    }

    public class UdpServer<T> : UdpServer
    {
        public new T GetExtra(IntPtr connId)
        {
            return base.GetExtra<T>(connId);
        }

        public bool SetExtra(IntPtr connId, T obj)
        {
            return base.SetExtra(connId, obj);
        }
    }

    public class UdpServer: ConnectionExtra
    {
        protected bool IsCreate = false;
        protected IntPtr _pServer = IntPtr.Zero;
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
        /// <summary>
        /// 服务器ip
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort Port { get; set; }

        // 是否启动
        public bool IsStarted
        {
            get
            {
                if (pServer == IntPtr.Zero)
                {
                    return false;
                }
                return Sdk.HP_Server_HasStarted(pServer);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public ServiceState State
        {
            get
            {
                return Sdk.HP_Server_GetState(pServer);
            }

        }

        /// <summary>
        /// 连接数
        /// </summary>
        public uint ConnectionCount
        {
            get
            {
                return Sdk.HP_Server_GetConnectionCount(pServer);
            }

        }

        /// <summary>
        /// 检测是否为安全连接（SSL/HTTPS）
        /// </summary>
        public bool IsSecure
        {
            get
            {
                return Sdk.HP_Server_IsSecure(pServer);
            }
        }
        /// <summary>
        /// 设置最大连接数（组件会根据设置值预分配内存，因此需要根据实际情况设置，不宜过大）
        /// </summary>
        public uint MaxConnectionCount
        {
            get
            {
                return Sdk.HP_Server_GetMaxConnectionCount(pServer);
            }
            set
            {
                Sdk.HP_Server_SetMaxConnectionCount(pServer, value);
            }
        }
        /// <summary>
        /// 获取错误码
        /// </summary>
        public SocketError ErrorCode
        {
            get
            {
                return Sdk.HP_Server_GetLastError(pServer);
            }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get
            {
                return Sdk.GetHPSocketVersion();
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                IntPtr ptr = Sdk.HP_Server_GetLastErrorDesc(pServer);
                string desc = Marshal.PtrToStringUni(ptr);
                return desc;
            }
        }
        /// <summary>
        /// 获取或设置数据发送策略
        /// </summary>
        public SendPolicy SendPolicy
        {
            get
            {
                return Sdk.HP_Server_GetSendPolicy(pServer);
            }
            set
            {
                Sdk.HP_Server_SetSendPolicy(pServer, value);
            }
        }
        /// <summary>
        /// 读取或设置是否标记静默时间（设置为 TRUE 时 DisconnectSilenceConnections() 和 GetSilencePeriod() 才有效，默认：FALSE）
        /// </summary>
        public bool MarkSilence
        {
            get
            {
                return Sdk.HP_Server_IsMarkSilence(pServer);
            }
            set
            {
                Sdk.HP_Server_SetMarkSilence(pServer, value);
            }
        }
        /// <summary>
        /// 读取或设置 Socket 缓存对象锁定时间（毫秒，在锁定期间该 Socket 缓存对象不能被获取使用）
        /// </summary>
        public uint FreeSocketObjLockTime
        {
            get
            {
                return Sdk.HP_Server_GetFreeSocketObjLockTime(pServer);
            }
            set
            {
                Sdk.HP_Server_SetFreeSocketObjLockTime(pServer, value);
            }
        }
        /// <summary>
        /// 读取或设置 Socket 缓存池大小（通常设置为平均并发连接数量的 1/3 - 1/2）
        /// </summary>
        public uint FreeSocketObjPool
        {
            get
            {
                return Sdk.HP_Server_GetFreeSocketObjPool(pServer);
            }
            set
            {
                Sdk.HP_Server_SetFreeSocketObjPool(pServer, value);
            }
        }
        /// <summary>
        /// 读取或设置内存块缓存池大小（通常设置为 Socket 缓存池大小的 2 - 3 倍）
        /// </summary>
        public uint FreeBufferObjPool
        {
            get
            {
                return Sdk.HP_Server_GetFreeBufferObjPool(pServer);
            }
            set
            {
                Sdk.HP_Server_SetFreeBufferObjPool(pServer, value);
            }
        }
        /// <summary>
        /// 读取或设置内存块缓存池大小（通常设置为 Socket 缓存池大小的 2 - 3 倍）
        /// </summary>
        public uint FreeSocketObjHold
        {
            get
            {
                return Sdk.HP_Server_GetFreeSocketObjHold(pServer);
            }
            set
            {
                Sdk.HP_Server_SetFreeSocketObjHold(pServer, value);
            }
        }

        /// <summary>
        /// 读取或设置内存块缓存池回收阀值（通常设置为内存块缓存池大小的 3 倍）
        /// </summary>
        public uint FreeBufferObjHold
        {
            get
            {
                return Sdk.HP_Server_GetFreeBufferObjHold(pServer);
            }
            set
            {
                Sdk.HP_Server_SetFreeBufferObjHold(pServer, value);
            }
        }
        /// <summary>
        /// 读取或设置工作线程数量（通常设置为 2 * CPU + 2）
        /// </summary>
        public uint WorkerThreadCount
        {
            get
            {
                return Sdk.HP_Server_GetWorkerThreadCount(pServer);
            }
            set
            {
                Sdk.HP_Server_SetWorkerThreadCount(pServer, value);
            }
        }
        /// <summary>
        /// 获取，设置数据报文最大长度
        /// </summary>
        public uint MaxDatagramSize
        {
            get
            {
                return Sdk.HP_UdpServer_GetMaxDatagramSize(pServer);
            }
            set
            {
                Sdk.HP_UdpServer_SetMaxDatagramSize(pServer, value);
            }
        }
        /// <summary>
        /// 获取，设置Receive预投递数量
        /// </summary>
        public uint PostReceiveCount
        {
            get
            {
                return Sdk.HP_UdpServer_GetPostReceiveCount(pServer);
            }
            set
            {
                Sdk.HP_UdpServer_SetPostReceiveCount(pServer, value);
            }
        }
        /// <summary>
        /// 获取心跳检查次数
        /// 设置监测包尝试次数（0 则不发送监测跳包，如果超过最大尝试次数则认为已断线）
        /// </summary>
        public uint DetectAttempt
        {
            get
            {
                return Sdk.HP_UdpServer_GetDetectAttempts(pServer);
            }
            set
            {
                Sdk.HP_UdpServer_SetDetectAttempts(pServer, value);
            }
        }
        /// <summary>
        /// 获取心跳检测间隔
        /// 设置监测包发送间隔（秒，0 不发送监测包）
        /// </summary>
        public uint DetectInterval
        {
            get
            {
                return Sdk.HP_UdpServer_GetDetectInterval(pServer);
            }
            set
            {
                Sdk.HP_UdpServer_SetDetectInterval(pServer, value);
            }
        }

        #region 事件定义
        /// <summary>
        /// 连接到达事件
        /// </summary>
        public event UdpServerEvent.OnAcceptEventHandler OnAccept;
        /// <summary>
        /// 数据包发送事件
        /// </summary>
        public event UdpServerEvent.OnSendEventHandler OnSend;
        /// <summary>
        /// 准备监听了事件
        /// </summary>
        public event UdpServerEvent.OnPrepareListenEventHandler OnPrepareListen;
        /// <summary>
        /// 数据到达事件
        /// </summary>
        public event UdpServerEvent.OnReceiveEventHandler OnReceive;
        /// <summary>
        /// 数据到达事件(指针数据)
        /// </summary>
        public event UdpServerEvent.OnPointerDataReceiveEventHandler OnPointerDataReceive;
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        public event UdpServerEvent.OnCloseEventHandler OnClose;
        /// <summary>
        /// 服务器关闭事件
        /// </summary>
        public event UdpServerEvent.OnShutdownEventHandler OnShutdown;
        /// <summary>
        /// 握手成功事件
        /// </summary>
        public event UdpServerEvent.OnHandShakeEventHandler OnHandShake;
        #endregion
        #region 回调函数
        protected Sdk.OnPrepareListen _OnPrepareListen = null;
        protected Sdk.OnAccept _OnAccept = null;
        protected Sdk.OnReceive _OnReceive = null;
        protected Sdk.OnSend _OnSend = null;
        protected Sdk.OnClose _OnClose = null;
        protected Sdk.OnShutdown _OnShutdown = null;
        protected Sdk.OnHandShake _OnHandShake = null;
        protected virtual void SetCallback()
        {
            _OnPrepareListen = new Sdk.OnPrepareListen(SDK_OnPrepareListen);
            _OnAccept = new Sdk.OnAccept(SDK_OnAccept);
            _OnSend = new Sdk.OnSend(SDK_OnSend);
            _OnReceive = new Sdk.OnReceive(SDK_OnReceive);
            _OnClose = new Sdk.OnClose(SDK_OnClose);
            _OnShutdown = new Sdk.OnShutdown(SDK_OnShutdown);
            _OnHandShake = new Sdk.OnHandShake(SDK_OnHandShake);

            Sdk.HP_Set_FN_Server_OnPrepareListen(pListener, _OnPrepareListen);
            Sdk.HP_Set_FN_Server_OnAccept(pListener, _OnAccept);
            Sdk.HP_Set_FN_Server_OnSend(pListener, _OnSend);
            Sdk.HP_Set_FN_Server_OnReceive(pListener, _OnReceive);
            Sdk.HP_Set_FN_Server_OnClose(pListener, _OnClose);
            Sdk.HP_Set_FN_Server_OnShutdown(pListener, _OnShutdown);
            Sdk.HP_Set_FN_Server_OnHandShake(pListener, _OnHandShake);
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
        #endregion

        public UdpServer()
        {
            CreateListener();
        }
        ~UdpServer()
        {

        }
        /// <summary>
        /// 创建socket监听&服务组件
        /// </summary>
        /// <returns></returns>
        protected virtual bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pServer != IntPtr.Zero)
            {
                return false;
            }
            pListener = Sdk.Create_HP_UdpServerListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }
            pServer = Sdk.Create_HP_UdpServer(pListener);
            if (pServer == IntPtr.Zero)
            {
                return false;
            }
            IsCreate = true;
            return false;
        }
        /// <summary>
        /// 终止服务并释放资源
        /// </summary>
        public virtual void Destroy()
        {
            Stop();
            if (pServer!=IntPtr.Zero)
            {
                Sdk.Destroy_HP_UdpServer(pServer);
            }
            if (pListener!=IntPtr.Zero)
            {
                Sdk.Destroy_HP_UdpServerListener(pListener);
            }
            IsCreate = false;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Start()
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

            return Sdk.HP_Server_Start(pServer, IpAddress, Port);
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (IsStarted == false)
            {
                return false;
            }

            return Sdk.HP_Server_Stop(pServer);
        }
        /// <summary>
        /// 断开与某个客户的连接
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bForce">是否强制断开</param>
        /// <returns></returns>
        public bool Disconnect(IntPtr connId, bool force = true)
        {
            return Sdk.HP_Server_Disconnect(pServer, connId, force);
        }
        /// <summary>
        /// 断开超过指定时间的连接
        /// </summary>
        /// <param name="period">毫秒</param>
        /// <param name="force">强制</param>
        /// <returns></returns>
        public bool DisconnectLongConnections(uint period, bool force = true)
        {
            return Sdk.HP_Server_DisconnectLongConnections(pServer, period, force);
        }
        /// <summary>
        /// 断开超过指定时长的静默连接
        /// </summary>
        /// <param name="period">毫秒</param>
        /// <param name="force">强制</param>
        /// <returns></returns>
        public bool DisconnectSilenceConnections(uint period, bool force = true)
        {
            return Sdk.HP_Server_DisconnectSilenceConnections(pServer, period, force);
        }
        /// <summary>
        /// 获取所有连接,未获取到连接返回null
        /// </summary>
        /// <returns></returns>
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
                if (Sdk.HP_Server_GetAllConnectionIDs(pServer, arr, ref count))
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
        /// <summary>
        /// 获取指定连接的连接时长（毫秒）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool GetConnectPeriod(IntPtr connId, ref uint period)
        {
            return Sdk.HP_Server_GetConnectPeriod(pServer, connId, ref period);
        }
        /// <summary>
        /// 获取某个连接静默时间（毫秒）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public bool GetSilencePeriod(IntPtr connId, ref uint period)
        {
            return Sdk.HP_Server_GetSilencePeriod(pServer, connId, ref period);
        }
        /// <summary>
        /// 获取某个连接的本地地址信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool GetLocalAddress(IntPtr connId, ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = Sdk.HP_Server_GetLocalAddress(pServer, connId, sb, ref ipLength, ref port) && ipLength > 0;
            if (ret == true)
            {
                ip = sb.ToString();
            }

            return ret;
        }
        /// <summary>
        /// 获取某个连接的远程地址信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool GetRemoteAddress(IntPtr connId, ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = Sdk.HP_Server_GetRemoteAddress(pServer, connId, sb, ref ipLength, ref port) && ipLength > 0;
            if (ret == true)
            {
                ip = sb.ToString();
            }

            return ret;
        }
        /// <summary>
        /// 获取连接中未发出数据的长度
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool GetPendingDataLength(IntPtr connId, ref int length)
        {
            return Sdk.HP_Server_GetPendingDataLength(pServer, connId, ref length);
        }
        /// <summary>
        /// 获取监听socket的地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool GetListenAddress(ref string ip, ref ushort port)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = Sdk.HP_Server_GetListenAddress(pServer, sb, ref ipLength, ref port);
            if (ret == true)
            {
                ip = sb.ToString();
            }
            return ret;
        }

        #region Send Message
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Send(IntPtr connId, byte[] bytes, int size)
        {
            return Sdk.HP_Server_Send(pServer, connId, bytes, size);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bufferPtr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Send(IntPtr connId, IntPtr bufferPtr, int size)
        {
            return Sdk.HP_Server_Send(pServer, connId, bufferPtr, size);
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bytes"></param>
        /// <param name="offset">针对bytes的偏移</param>
        /// <param name="size">发多大</param>
        /// <returns></returns>
        public bool Send(IntPtr connId, byte[] bytes, int offset, int size)
        {
            return Sdk.HP_Server_SendPart(pServer, connId, bytes, size, offset);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bufferPtr"></param>
        /// <param name="offset">针对bufferPtr的偏移</param>
        /// <param name="size">发多大</param>
        /// <returns></returns>
        public bool Send(IntPtr connId, IntPtr bufferPtr, int offset, int size)
        {
            return Sdk.HP_Server_SendPart(pServer, connId, bufferPtr, size, offset);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bufferPtr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Send<T>(IntPtr connId, T obj)
        {
            byte[] buffer = StructureToByte<T>(obj);
            return Send(connId, buffer, buffer.Length);
        }

        /// <summary>
        /// 序列化对象后发送数据,序列化对象所属类必须标记[Serializable]
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bufferPtr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool SendBySerializable(IntPtr connId, object obj)
        {
            byte[] buffer = ObjectToBytes(obj);
            return Send(connId, buffer, buffer.Length);
        }

        /// <summary>
        /// 发送多组数据
        /// 向指定连接发送多组数据
        /// TCP - 顺序发送所有数据包
        /// </summary>
        /// <param name="connId">连接 ID</param>
        /// <param name="pBuffers">发送缓冲区数组</param>
        /// <param name="iCount">发送缓冲区数目</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        public bool SendPackets(IntPtr connId, WSABUF[] pBuffers, int count)
        {
            return Sdk.HP_Server_SendPackets(pServer, connId, pBuffers, count);
        }

        /// <summary>
        /// 发送多组数据
        /// 向指定连接发送多组数据
        /// TCP - 顺序发送所有数据包
        /// </summary>
        /// <param name="connId">连接 ID</param>
        /// <param name="pBuffers">发送缓冲区数组</param>
        /// <param name="iCount">发送缓冲区数目</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
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

        /// <summary>
        /// 名称：发送小文件
        /// 描述：向指定连接发送 4096 KB 以下的小文件
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="head">头部附加数据</param>
        /// <param name="tail">尾部附加数据</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        public bool SendSmallFile(IntPtr connId, string filePath, ref WSABUF head, ref WSABUF tail)
        {
            return Sdk.HP_TcpServer_SendSmallFile(pServer, connId, filePath, ref head, ref tail);
        }

        /// <summary>
        /// 名称：发送小文件
        /// 描述：向指定连接发送 4096 KB 以下的小文件
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="head">头部附加数据,可以为null</param>
        /// <param name="tail">尾部附加数据,可以为null</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        public bool SendSmallFile(IntPtr connId, string filePath, byte[] head, byte[] tail)
        {
            IntPtr pHead = IntPtr.Zero;
            IntPtr pTail = IntPtr.Zero;
            WSABUF wsaHead = new WSABUF() { Length = 0, Buffer = pHead };
            WSABUF wsatail = new WSABUF() { Length = 0, Buffer = pTail };
            if (head != null)
            {
                wsaHead.Length = head.Length;
                wsaHead.Buffer = Marshal.UnsafeAddrOfPinnedArrayElement(head, 0);
            }

            if (tail != null)
            {
                wsaHead.Length = tail.Length;
                wsaHead.Buffer = Marshal.UnsafeAddrOfPinnedArrayElement(tail, 0);
            }

            return SendSmallFile(connId, filePath, ref wsaHead, ref wsatail);
        }

        /// <summary>
        /// 名称：发送小文件
        /// 描述：向指定连接发送 4096 KB 以下的小文件
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="head">头部附加数据,可以为null</param>
        /// <param name="tail">尾部附加数据,可以为null</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        public bool SendSmallFile<T1, T2>(IntPtr connId, string filePath, T1 head, T2 tail)
        {

            byte[] headBuffer = null;
            if (head != null)
            {
                headBuffer = StructureToByte<T1>(head);
            }

            byte[] tailBuffer = null;
            if (tail != null)
            {
                tailBuffer = StructureToByte<T2>(tail);
            }
            return SendSmallFile(connId, filePath, headBuffer, tailBuffer);
        }
        #endregion

        #region 基础方法
        /// <summary>
        /// 由结构体转换为byte数组
        /// </summary>
        public byte[] StructureToByte<T>(T structure)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, bufferIntPtr, true);
                Marshal.Copy(bufferIntPtr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferIntPtr);
            }
            return buffer;
        }

        /// <summary>
        /// 由byte数组转换为结构体
        /// </summary>
        public T ByteToStructure<T>(byte[] dataBuffer)
        {
            object structure = null;
            int size = Marshal.SizeOf(typeof(T));
            IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(dataBuffer, 0, allocIntPtr, size);
                structure = Marshal.PtrToStructure(allocIntPtr, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(allocIntPtr);
            }
            return (T)structure;
        }

        /// <summary>
        /// 对象序列化成byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// byte[]序列化成对象
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public object BytesToObject(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// byte[]转结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public T BytesToStruct<T>(byte[] bytes)
        {
            Type strcutType = typeof(T);
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return (T)Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        #endregion
    }
}

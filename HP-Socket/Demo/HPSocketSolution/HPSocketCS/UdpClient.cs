using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HPSocketCS
{
    public class UdpClientEvent
    {
        public delegate HandleResult OnPrepareConnectEventHandler(UdpClient sender, IntPtr socket);
        public delegate HandleResult OnConnectEventHandler(UdpClient sender);
        public delegate HandleResult OnSendEventHandler(UdpClient sender, byte[] bytes);
        public delegate HandleResult OnReceiveEventHandler(UdpClient sender, byte[] bytes);
        public delegate HandleResult OnPointerDataReceiveEventHandler(UdpClient sender, IntPtr pData, int length);
        public delegate HandleResult OnCloseEventHandler(UdpClient sender, SocketOperation enOperation, int errorCode);
        public delegate HandleResult OnHandShakeEventHandler(UdpClient sender);
    }
    public class UdpClient<T> : UdpClient
    {
        public T GetExtra()
        {
            return base.GetExtra<T>();
        }

        public bool SetExtra(T obj)
        {
            return base.SetExtra(obj);
        }
    }
    public  class UdpClient
    {
        private IntPtr _pClient = IntPtr.Zero;

        public IntPtr pClient
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
        protected bool isCreate = false;
        ConnectionExtra ExtraData = new ConnectionExtra();

        #region Public Property
        /// <summary>
        /// 是否启动
        /// </summary>
        public bool IsStarted
        {
            get
            {
                if (pClient==IntPtr.Zero)
                {
                    return false;
                }
                return Sdk.HP_Client_HasStarted(pClient);
            }
        }
        /// <summary>
        /// 检测是否为安全连接（SSL/HTTPS）
        /// </summary>
        public bool IsSecure
        {
            get
            {
                return Sdk.HP_Client_IsSecure(pClient);
            }
        }
        /// <summary>
        /// 服务状态
        /// </summary>
        public ServiceState State
        {
            get
            {
                return Sdk.HP_Client_GetState(pClient);
            }
        }
        /// <summary>
        /// 错误码
        /// </summary>
        public SocketError ErrorCode
        {
            get
            {
                return Sdk.HP_Client_GetLastError(pClient);
            }
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                IntPtr ptr = Sdk.HP_Client_GetLastErrorDesc(pClient);
                string desc = Marshal.PtrToStringUni(ptr);
                return desc;
            }
        }
        /// <summary>
        /// 获取该组件对象的连接Id
        /// </summary>
        public IntPtr ConnectionId
        {
            get
            {
                return Sdk.HP_Client_GetConnectionID(pClient);
            }
        }
        /// <summary>
        /// 读取或设置内存块缓存池大小（通常设置为 -> PUSH 模型：5 - 10；PULL 模型：10 - 20 ）
        /// </summary>
        public uint FreeBufferPoolSize
        {
            get
            {
                return Sdk.HP_Client_GetFreeBufferPoolSize(pClient);
            }
            set
            {
                Sdk.HP_Client_SetFreeBufferPoolSize(pClient, value);
            }
        }
        /// <summary>
        ///  读取或设置内存块缓存池回收阀值（通常设置为内存块缓存池大小的 3 倍）
        /// </summary>
        public uint FreeBufferPoolHold
        {
            get
            {
                return Sdk.HP_Client_GetFreeBufferPoolHold(pClient);
            }
            set
            {
                Sdk.HP_Client_SetFreeBufferPoolHold(pClient, value);
            }
        }
        /// <summary>
        /// 获取或设置数据报文最大长度
        /// </summary>
        public uint MaxDatagramSize
        {
            get
            {
                return Sdk.HP_UdpClient_GetMaxDatagramSize(pClient);
            }
            set
            {
                Sdk.HP_UdpClient_SetMaxDatagramSize(pClient, value);
            }
        }
        /// <summary>
        /// 获取或设置心跳检测次数
        /// </summary>
        public uint DetectAttempts
        {
            get
            {
                return Sdk.HP_UdpClient_GetDetectAttempts(pClient);
            }
            set
            {
                Sdk.HP_UdpClient_SetDetectAttempts(pClient, value);
            }
        }
        /// <summary>
        /// 获取或设置心跳检查间隔
        /// </summary>
        public uint DetectInterval
        {
            get
            {
                return Sdk.HP_UdpClient_GetDetectInterval(pClient);
            }
            set
            {
                Sdk.HP_UdpClient_SetDetectInterval(pClient, value);
            }
        }
        #endregion

        #region 回调函数定义
        protected Sdk.OnPrepareConnect _OnPrepareConnect = null;
        protected Sdk.OnConnect _OnConnect = null;
        protected Sdk.OnReceive _OnReceive = null;
        protected Sdk.OnSend _OnSend = null;
        protected Sdk.OnClose _OnClose = null;
        protected Sdk.OnHandShake _OnHandShake = null;
        #endregion

        #region Event
        /// <summary>
        /// 准备连接了事件
        /// </summary>
        public event UdpClientEvent.OnPrepareConnectEventHandler OnPrepareConnect;
        /// <summary>
        /// 连接事件
        /// </summary>
        public event UdpClientEvent.OnConnectEventHandler OnConnect;
        /// <summary>
        /// 数据发送事件
        /// </summary>
        public event UdpClientEvent.OnSendEventHandler OnSend;
        /// <summary>
        /// 数据到达事件
        /// </summary>
        public event UdpClientEvent.OnReceiveEventHandler OnReceive;
        /// <summary>
        /// 数据到达事件(指针数据)
        /// </summary>
        public event UdpClientEvent.OnPointerDataReceiveEventHandler OnPointerDataReceive;
        /// <summary>
        /// 连接关闭事件
        /// </summary>
        public event UdpClientEvent.OnCloseEventHandler OnClose;
        /// <summary>
        /// 握手事件
        /// </summary>
        public event UdpClientEvent.OnHandShakeEventHandler OnHandShake;
        #endregion

        public UdpClient()
        {
            CreateListener();
        }

        ~UdpClient()
        {
            Destroy();
        }
        /// <summary>
        /// 创建socket监听&服务组件
        /// </summary>
        /// <returns></returns>
        protected virtual bool CreateListener()
        {
            if (isCreate==true||pListener!=IntPtr.Zero||pClient!=IntPtr.Zero)
            {
                return false;
            }
            pListener = Sdk.Create_HP_UdpClientListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }
            pClient = Sdk.Create_HP_UdpClient(pListener);
            if (pClient == IntPtr.Zero)
            {
                return false;
            }
            isCreate = true;
            return true;
        }

        /// <summary>
        /// 终止服务并释放资源
        /// </summary>
        public virtual void Destroy()
        {
            Stop();
            if (pClient!=IntPtr.Zero)
            {
                Sdk.Destroy_HP_UdpClient(pClient);
            }
            if (pListener!=IntPtr.Zero)
            {
                Sdk.Destroy_HP_UdpClientListener(pListener);
            }
            isCreate = false;
        }
       
        /// <summary>
        /// 启动通讯组件并连接到服务器
        /// </summary>
        /// <param name="address">远程地址</param>
        /// <param name="port">端口号</param>
        /// <param name="async">是否异步连接</param>
        /// <returns></returns>
        public bool Connect(string address,ushort port,bool async = true)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("address is null");
            }
            if (port==0)
            {
                throw new Exception("port is zero");
            }
            if (IsStarted == true)
            {
                return false;
            }
            this.SetCallback();
            return Sdk.HP_Client_Start(pClient, address, port, async);
        }
        /// <summary>
        /// 启动通讯组件并连接到服务器
        /// </summary>
        /// <param name="address">远程地址</param>
        /// <param name="port">远程端口</param>
        /// <param name="bindAddress">本地绑定到哪个ip?,多ip下可以选择绑定到指定ip</param>
        /// <param name="async">是否异步</param>
        /// <returns></returns>
        public bool Connect(string address, ushort port, string bindAddress, bool async = true)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("address is null");
            }
            if (port == 0)
            {
                throw new Exception("port is zero");
            }
            if (IsStarted == true)
            {
                return false;
            }
            this.SetCallback();
            return Sdk.HP_Client_StartWithBindAddress(pClient, address, port, async, bindAddress);
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
            return Sdk.HP_Client_Stop(pClient);
        }

        public bool Send(byte[] bytes,int size)
        {
            return Sdk.HP_Client_Send(pClient, bytes, size);
        }
        public bool Send(byte[] bytes,int offset,int size)
        {
            return Sdk.HP_Client_SendPart(pClient, bytes, size, offset);
        }
        public bool Send(IntPtr bufferPtr, int offset, int size)
        {
            return Sdk.HP_Client_SendPart(pClient, bufferPtr, size, offset);
        }
        public bool Send(IntPtr bufferPtr,int size)
        {
            return Sdk.HP_Client_Send(pClient, bufferPtr, size);
        }
        public bool Send<T>(T obj)
        {
            byte[] buffer = StructureToByte<T>(obj);
            return Send(buffer, buffer.Length);
        }
        /// <summary>
        /// 发送多组数据
        /// 向指定连接发送多组数据
        /// TCP - 顺序发送所有数据包
        /// </summary>
        /// <param name="pBuffers">发送缓冲区数组</param>
        /// <param name="iCount">发送缓冲区数目</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        public bool SendPackets(WSABUF[] buffers,int count)
        {
            return Sdk.HP_Client_SendPackets(pClient, buffers, count);
        }
        /// <summary>
        /// 发送多组数据
        /// 向指定连接发送多组数据
        /// TCP - 顺序发送所有数据包
        /// </summary>
        /// <param name="objects">对象数组</param>
        /// <returns>TRUE.成功,FALSE.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
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
        /// <summary>
        /// 序列化对象后发送数据,序列化对象所属类必须标记[Serializable]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SendBySerializable(object obj)
        {
            byte[] buffer = ObjectToBytes(obj);
            return Send(buffer, buffer.Length);
        }


        /// <summary>
        /// 获取监听socket的地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool GetListenAddress(ref string ip,ref ushort port)
        {
            int ipLength = 40;
            StringBuilder sb = new StringBuilder(ipLength);
            bool ret = Sdk.HP_Client_GetLocalAddress(pClient, sb, ref ipLength, ref port);
            if (ret == true)
            {
                ip = sb.ToString();
            }
            return ret;
        }
        /// <summary>
        /// 获取连接的远程主机信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool GetRemoteHost(ref string host,ref ushort port)
        {
            int ipLength = 40;
            StringBuilder sb = new StringBuilder(ipLength);
            bool ret = Sdk.HP_Client_GetRemoteHost(pClient, sb, ref ipLength, ref port);
            if (ret)
            {
                host = ipLength.ToString();
            }
            return ret;
        }
        /// <summary>
        /// 获取连接中未发出的数据长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool GetPendingDataLength(ref int length)
        {
            return Sdk.HP_Client_GetPendingDataLength(pClient,ref length);
        }


        /// <summary>
        /// 设置附加数据
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool SetExtra(object newValue)
        {
            return ExtraData.SetExtra(pClient, newValue);
        }
        /// <summary>
        /// 获取附加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetExtra<T>()
        {
            return (T)ExtraData.GetExtra(pClient);
        }



        #region 回调函数处理
        /// <summary>
        /// 设置回调函数
        /// </summary>
        protected virtual void SetCallback()
        {
            _OnPrepareConnect = new Sdk.OnPrepareConnect(SDK_OnPrepareConnect);
            _OnConnect = new Sdk.OnConnect(SDK_OnConnect);
            _OnSend = new Sdk.OnSend(SDK_OnSend);
            _OnReceive = new Sdk.OnReceive(SDK_OnReceive);
            _OnClose = new Sdk.OnClose(SDK_OnClose);
            _OnHandShake = new Sdk.OnHandShake(SDK_OnHandShake);

            Sdk.HP_Set_FN_Client_OnPrepareConnect(pListener, _OnPrepareConnect);
            Sdk.HP_Set_FN_Client_OnConnect(pListener, _OnConnect);
            Sdk.HP_Set_FN_Client_OnSend(pListener, _OnSend);
            Sdk.HP_Set_FN_Client_OnReceive(pListener, _OnReceive);
            Sdk.HP_Set_FN_Client_OnClose(pListener, _OnClose);
            Sdk.HP_Set_FN_Client_OnHandShake(pListener, _OnHandShake);
        }

        private HandleResult SDK_OnHandShake(IntPtr pSender, IntPtr connId)
        {
            if (OnHandShake!=null)
            {
                return OnHandShake(this);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnClose(IntPtr pSender, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            if (OnClose!=null)
            {
                return OnClose(this, enOperation, errorCode);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnReceive(IntPtr pSender, IntPtr connId, IntPtr pData, int length)
        {
            if (OnPointerDataReceive!=null)
            {
                return OnPointerDataReceive(this, pData, length);
            }
            else if (OnReceive!=null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnReceive(this, bytes);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnSend(IntPtr pSender, IntPtr connId, IntPtr pData, int length)
        {
            if (OnSend!=null)
            {
                byte[] bytes = new byte[length];
                Marshal.Copy(pData, bytes, 0, length);
                return OnSend(this, bytes);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnConnect(IntPtr pSender, IntPtr connId)
        {
            if (OnConnect!=null)
            {
                return OnConnect(this);
            }
            return HandleResult.Ignore;
        }

        private HandleResult SDK_OnPrepareConnect(IntPtr pSender, IntPtr connId, IntPtr socket)
        {
            if (OnPrepareConnect!=null)
            {
                return OnPrepareConnect(this, socket);
            }
            return HandleResult.Ignore;
        }

        #endregion 
        #region Base Method
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

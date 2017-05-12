using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs
{
    /// <summary>
    /// 通信组件服务状态,用程序可以通过通信组件的 GetState() 方法获取组件当前服务状态
    /// </summary>
    public enum ServiceState
    {
        /// <summary>
        /// 正在启动
        /// </summary>
        Starting = 0,
        /// <summary>
        /// 已经启动
        /// </summary>
        Started = 1,
        /// <summary>
        /// 正在停止
        /// </summary>
        Stoping = 2,
        /// <summary>
        /// 已经启动
        /// </summary>
        Stoped = 3,
    }

    /// <summary>
    /// Socket 操作类型,应用程序的 OnErrror() 事件中通过该参数标识是哪种操作导致的错误
    /// </summary>
    public enum SocketOperation
    {
        Unknown = 0,    // Unknown
        Acccept = 1,    // Acccept
        Connnect = 2,   // Connnect
        Send = 3,       // Send
        Receive = 4,    // Receive
        Close = 5,    // Receive
    };

    /// <summary>
    /// 事件通知处理结果,事件通知的返回值，不同的返回值会影响通信组件的后续行为
    /// </summary>
    public enum HandleResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 忽略
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 2,
    };


    /// <summary>
    /// 名称：操作结果代码
    /// 描述：组件 Start() / Stop() 方法执行失败时，可通过 GetLastError() 获取错误代码
    /// </summary>
    public enum SocketError
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 当前状态不允许操作
        /// </summary>
        IllegalState = 1,
        /// <summary>
        /// 非法参数
        /// </summary>
        InvalidParam = 2,
        /// <summary>
        /// 创建 SOCKET 失败
        /// </summary>
        SocketCreate = 3,
        /// <summary>
        /// 绑定 SOCKET 失败
        /// </summary>
        SocketBind = 4,
        /// <summary>
        /// 设置 SOCKET 失败
        /// </summary>
        SocketPrepare = 5,
        /// <summary>
        /// 监听 SOCKET 失败
        /// </summary>
        SocketListen = 6,
        /// <summary>
        /// 创建完成端口失败
        /// </summary>
        CPCreate = 7,
        /// <summary>
        /// 创建工作线程失败
        /// </summary>
        WorkerThreadCreate = 8,
        /// <summary>
        /// 创建监测线程失败
        /// </summary>
        DetectThreadCreate = 9,
        /// <summary>
        /// 绑定完成端口失败
        /// </summary>
        SocketAttachToCP = 10,
        /// <summary>
        /// 连接服务器失败
        /// </summary>
        ConnectServer = 11,
        /// <summary>
        /// 网络错误
        /// </summary>
        Network = 12,
        /// <summary>
        /// 数据处理错误
        /// </summary>
        DataProc = 13,
        /// <summary>
        /// 数据发送失败
        /// </summary>
        DataSend = 14,

        /***** SSL Socket 扩展操作结果代码 *****/
        /// <summary>
        /// SSL 环境未就绪
        /// </summary>
        SSLEnvNotReady = 101,
    };

    /// <summary>
    /// 数据抓取结果,数据抓取操作的返回值
    /// </summary>
    public enum FetchResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 抓取长度过大
        /// </summary>
        LengthTooLong = 1,
        /// <summary>
        /// 找不到 ConnID 对应的数据
        /// </summary>
        DataNotFound = 2,
    };

    /// <summary>
    /// 发送策略
    /// </summary>
    public enum SendPolicy
    {
        /// <summary>
        /// 打包模式（默认）
        /// </summary>
        Pack = 0,
        /// <summary>
        /// 安全模式
        /// </summary>
        Safe = 1,
        /// <summary>
        /// 直接模式
        /// </summary>
        Direct = 2,
    };

    /// <summary>
    /// 播送模式  UDP 组件的播送模式（组播或广播）
    /// </summary>
    public enum CastMode
    {
        /// <summary>
        /// 组播
        /// </summary>
        Multicast = 0,
        /// <summary>
        /// 广播
        /// </summary>
        Broadcast = 1,

    }
}

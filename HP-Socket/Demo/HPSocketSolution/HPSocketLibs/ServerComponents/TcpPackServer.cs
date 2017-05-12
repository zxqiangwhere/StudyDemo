using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    //public class TcpPackServer<T> : TcpPackServer
    //{
    //    public new T GetExtra(IntPtr connId)
    //    {
    //        return base.GetExtra<T>(connId);
    //    }

    //    public bool SetExtra(IntPtr connId, T obj)
    //    {
    //        return base.SetExtra(connId, obj);
    //    }
    //}

    public class TcpPackServer : TcpServer, ITcpPackServer
    {
        public uint MaxPackSize
        {
            get
            {
                return SdkFunctions.HP_TcpPackServer_GetMaxPackSize(pServer);
            }
            set
            {
                SdkFunctions.HP_TcpPackServer_SetMaxPackSize(pServer, value);
            }
        }

        public ushort PackHeaderFlag
        {
            get
            {
                return SdkFunctions.HP_TcpPackServer_GetPackHeaderFlag(pServer);
            }
            set
            {
                SdkFunctions.HP_TcpPackServer_SetPackHeaderFlag(pServer, value);
            }
        }
        protected override bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pServer != IntPtr.Zero)
            {
                return false;
            }

            pListener = SdkFunctions.Create_HP_TcpPackServerListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }
            pServer = SdkFunctions.Create_HP_TcpPackServer(pListener);
            if (pServer == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            return true;
        }

        public override void Destroy()
        {
            Stop();

            if (pServer != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPackServer(pServer);
                pServer = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPackServerListener(pListener);
                pListener = IntPtr.Zero;
            }

            IsCreate = false;
        }
    }
}

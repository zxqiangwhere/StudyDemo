using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public class TcpPullServer : TcpServer, ITcpPullServer
    {
        public delegate HandleResult OnReceiveEventHandler(IntPtr connId, int length);
        public new event OnReceiveEventHandler OnReceive;

        new SdkFunctions.OnPullReceive _OnReceive = null;
        protected override bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pServer != IntPtr.Zero)
            {
                return false;
            }

            pListener = SdkFunctions.Create_HP_TcpPullServerListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pServer = SdkFunctions.Create_HP_TcpPullServer(pListener);
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
                SdkFunctions.Destroy_HP_TcpPullServer(pServer);
                pServer = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPullServerListener(pListener);
                pListener = IntPtr.Zero;
            }
            IsCreate = false;
        }

        public FetchResult Fetch( IntPtr connId, IntPtr pBuffer, int length)
        {
            return SdkFunctions.HP_TcpPullServer_Fetch(pServer, connId, pBuffer, length);
        }

        public FetchResult Peek( IntPtr connId, IntPtr pBuffer, int length)
        {
            return SdkFunctions.HP_TcpPullServer_Peek(pServer, connId, pBuffer, length);
        }
        protected override void SetCallback()
        {
            _OnReceive = new SdkFunctions.OnPullReceive(SDK_OnReceive);
            SdkFunctions.HP_Set_FN_Server_OnPullReceive(pListener, _OnReceive);
            base.SetCallback();
        }
        protected HandleResult SDK_OnReceive(IntPtr pSender, IntPtr connId, int length)
        {
            if (OnReceive != null)
            {
                return OnReceive(connId, length);
            }
            return HandleResult.Ignore;
        }
    }
}

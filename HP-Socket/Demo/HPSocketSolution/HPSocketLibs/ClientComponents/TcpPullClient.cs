using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public class TcpPullClient : TcpClient, ITcpPullClient
    {
        public event ClientEvent.OnTcpPullReceiveEventHandler OnPullReceive;
        new SdkFunctions.OnPullReceive _OnReceive = null;
        public override void Destroy()
        {
            Stop();

            if (pClient != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPullClient(pClient);
                pClient = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPullClientListener(pListener);
                pListener = IntPtr.Zero;
            }

            IsCreate = false;
        }
        public override bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pClient != IntPtr.Zero)
            {
                return false;
            }

            pListener = SdkFunctions.Create_HP_TcpPullClientListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pClient = SdkFunctions.Create_HP_TcpPullClient(pListener);
            if (pClient == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            return true;
        }
        protected override void SetCallback()
        {
            _OnReceive = new SdkFunctions.OnPullReceive(SDK_OnReceive);
            SdkFunctions.HP_Set_FN_Client_OnPullReceive(pListener, _OnReceive);
            base.SetCallback();
        }
        protected HandleResult SDK_OnReceive(IntPtr pSender, IntPtr pClient, int length)
        {
            if (OnPullReceive != null)
            {
                return OnPullReceive(this, length);
            }

            return HandleResult.Ignore;
        }
        public FetchResult Fetch(IntPtr pClient, IntPtr pBuffer, int length)
        {
            return SdkFunctions.HP_TcpPullClient_Fetch(pClient, pBuffer, length);
        }

        public FetchResult Peek(IntPtr pClient, IntPtr pBuffer, int length)
        {
            return SdkFunctions.HP_TcpPullClient_Peek(pClient, pBuffer, length);
        }
    }
}

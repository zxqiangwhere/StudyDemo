using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public class TcpPackClient : TcpClient, ITcpPackClient
    {
        public override void Destroy()
        {
            Stop();

            if (pClient != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPackClient(pClient);
                pClient = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpPackClientListener(pListener);
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

            pListener = SdkFunctions.Create_HP_TcpPackClientListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pClient = SdkFunctions.Create_HP_TcpPackClient(pListener);
            if (pClient == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            return true;
        }
        public uint MaxPackSize
        {
            get
            {
                return SdkFunctions.HP_TcpPackClient_GetMaxPackSize(pClient);
            }
            set
            {
                SdkFunctions.HP_TcpPackClient_SetMaxPackSize(pClient, value);
            }
        }

        public ushort PackHeaderFlag
        {
            get
            {
                return SdkFunctions.HP_TcpPackClient_GetPackHeaderFlag(pClient);
            }
            set
            {
                SdkFunctions.HP_TcpPackClient_SetPackHeaderFlag(pClient, value);
            }
        }
    }
}

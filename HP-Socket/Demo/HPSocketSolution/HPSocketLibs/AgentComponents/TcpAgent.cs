using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.AgentComponents
{
    public class TcpAgent : ServerComponents.TcpServer, ITcpAgent
    {
        protected IntPtr _pAgent = IntPtr.Zero;

        protected IntPtr pAgent
        {
            get
            {
                return _pAgent;
            }

            set
            {
                _pAgent = value;
            }
        }
        protected override bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pAgent != IntPtr.Zero)
            {
                return false;
            }

            pListener = SdkFunctions.Create_HP_TcpAgentListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pAgent = SdkFunctions.Create_HP_TcpAgent(pListener);
            if (pAgent == IntPtr.Zero)
            {
                return false;
            }

            IsCreate = true;

            pServer = pAgent;

            return true;
        }

        public override void Destroy()
        {
            Stop();

            if (pAgent != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpAgent(pAgent);
                pAgent = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                SdkFunctions.Destroy_HP_TcpAgentListener(pListener);
                pListener = IntPtr.Zero;
            }

            IsCreate = false;
        }

        public bool IsReuseAddress
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

        public bool Connect(string pszBindAddress, ushort usPort, ref IntPtr pdwConnID)
        {
            return SdkFunctions.HP_Agent_Connect(pAgent, pszBindAddress, usPort, ref pdwConnID);
        }
        public IntPtr Connect(string address, ushort port)
        {
            IntPtr connId = IntPtr.Zero;
            SdkFunctions.HP_Agent_Connect(pAgent, address, port, ref connId);
            return connId;
        }

        public IntPtr Connect(EndPoint endpoint)
        {
            IntPtr connId = IntPtr.Zero;
            SdkFunctions.HP_Agent_Connect(pAgent, endpoint.Address, endpoint.Port, ref connId);
            return connId;
        }

        public bool GetRemoteHost(IntPtr dwConnID, string lpszAddress, ref ushort pusPort)
        {
            int ipLength = 40;

            StringBuilder sb = new StringBuilder(ipLength);

            bool ret = SdkFunctions.HP_Agent_GetRemoteHost(pAgent, dwConnID, sb, ref ipLength, ref pusPort);
            if (ret == true)
            {
                lpszAddress = sb.ToString();
            }
            return ret;
        }

        public bool Start(string pszBindAddress, bool bAsyncConnect)
        {
            if (string.IsNullOrEmpty(pszBindAddress) == true)
            {
                throw new Exception("address is null");
            }

            if (IsCreate == false)
            {
                return false;
            }

            if (IsStarted == true)
            {
                return false;
            }

            SetCallback();

            return SdkFunctions.HP_Agent_Start(pAgent, pszBindAddress, bAsyncConnect);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.AgentComponents
{
    public interface IAgent:ServerComponents.IComplexSocket
    {
        bool Start(string pszBindAddress, bool bAsyncConnect);
        bool Connect(string pszBindAddress, ushort usPort, ref IntPtr pdwConnID);
        bool GetRemoteHost(IntPtr dwConnID, string lpszAddress, ref ushort pusPort);
    }
}

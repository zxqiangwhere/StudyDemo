using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public interface ITcpPullServer
    {
        FetchResult Fetch(IntPtr connId, IntPtr pBuffer, int length);
        FetchResult Peek(IntPtr connId, IntPtr pBuffer, int length);
    }
}

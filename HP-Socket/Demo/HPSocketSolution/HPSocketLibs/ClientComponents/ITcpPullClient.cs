using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public interface ITcpPullClient : ITcpClient
    {
        FetchResult Fetch(IntPtr pClient, IntPtr pBuffer, int length);
        FetchResult Peek(IntPtr pClient, IntPtr pBuffer, int length);
    }
}

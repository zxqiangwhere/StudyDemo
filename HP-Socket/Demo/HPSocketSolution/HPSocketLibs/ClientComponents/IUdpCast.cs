using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public interface IUdpCast : IClient
    {
        uint MaxDatagramSize { get; set; }
        bool IsReuseAddress { get; }
        bool IsMultiCastLoop { get; }
        CastMode CastMode { get; set; }
        int MultiCastTtl { get; set; }

        void SetReuseAddress(bool value);
        void SetMultiCastLoop(bool value);
        bool GetRemoteAddress(IntPtr connId, ref string ip, ref ushort port);
    }
}

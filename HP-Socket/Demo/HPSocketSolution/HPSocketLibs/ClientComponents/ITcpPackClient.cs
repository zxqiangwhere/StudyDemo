using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public interface ITcpPackClient : ITcpClient
    {
        uint MaxPackSize { get; set; }
        ushort PackHeaderFlag { get; set; }
    }
}

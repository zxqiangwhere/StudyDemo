using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public interface ITcpPackServer
    {
        uint MaxPackSize { get; set; }
        ushort PackHeaderFlag { get; set; }
    }
}

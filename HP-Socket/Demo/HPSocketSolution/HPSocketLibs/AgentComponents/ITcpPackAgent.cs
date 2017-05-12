using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.AgentComponents
{
    public interface ITcpPackAgent:ITcpAgent
    {
        uint MaxPackSize { get; set; }
        ushort PackHeaderFlag { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public interface IUdpServer:IServer
    {
        uint MaxDatagramSize { get; set; }
        uint DetectAttempts { get; set; }
        uint DetectInterval { get; set; }
        uint PostReceiveCount { get; set; }
    }
}

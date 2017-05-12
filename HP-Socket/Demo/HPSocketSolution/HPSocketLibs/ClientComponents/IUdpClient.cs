using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ClientComponents
{
    public interface IUdpClient : IClient
    {
        uint MaxDatagramSize { get; set; }
        uint DetectAttempts { get; set; }
        uint DetectInterval { get; set; }
    }
}

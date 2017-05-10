using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ClientInfo
    {
        public IntPtr ConnId { get; set; }
        public string IpAddress { get; set; }
        public ushort Port { get; set; }
        public PkgInfo PkgInfo { get; set; }
    }
}

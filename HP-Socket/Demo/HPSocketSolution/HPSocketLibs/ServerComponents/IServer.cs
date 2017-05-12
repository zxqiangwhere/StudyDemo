using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs.ServerComponents
{
    public interface IServer: IComplexSocket
    {
        bool Start( string pszBindAddress, ushort usPort);
        bool GetListenAddress(ref string ip, ref ushort port);
    }
}

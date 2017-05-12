using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct in_addr
    {
        public ulong S_addr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct sockaddr_in
    {
        public short sin_family;
        public ushort sin_port;
        public in_addr sin_addr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] sLibNamesin_zero;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct WSABUF
    {
        public int Length;
        public IntPtr Buffer;
    }
}

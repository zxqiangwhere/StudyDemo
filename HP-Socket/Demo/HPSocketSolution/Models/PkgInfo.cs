using System.Runtime.InteropServices;

namespace Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class PkgInfo
    {
        public bool IsHeader;
        public int Length;
    }
}
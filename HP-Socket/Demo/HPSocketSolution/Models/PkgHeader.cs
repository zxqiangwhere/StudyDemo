using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class PkgHeader
    {
        public int Id;
        public int BodySize;
    }
}

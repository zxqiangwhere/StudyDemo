﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [StructLayout(LayoutKind.Sequential)]
    public class test
    {
        public int age { get; set; }
        public int heght { get; set; }
    }
}

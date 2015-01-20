using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BadApple.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AMDevice
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        internal byte[] unknown0;
        internal uint device_id;
        internal uint product_id;
        public string serial;
        internal uint unknown1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        internal byte[] unknown2;
        internal uint lockdown_conn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal byte[] unknown3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x61)]
        internal byte[] unknown4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        internal byte[] unknown5;
    }
}

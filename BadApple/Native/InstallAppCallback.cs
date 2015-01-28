using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BadApple.Native
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InstallAppCallback(IntPtr CFDictionaryRef);
}

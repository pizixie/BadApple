using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BadApple.Native
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DeviceNotificationCallback(ref AMDeviceNotificationCallbackInfo callback_info);
}


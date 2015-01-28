using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BadApple.Native
{
    public static class iTunesMobileDevice
    {
        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceConnect(IntPtr device);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceDisconnect(IntPtr device);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceIsPaired(IntPtr device);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceValidatePairing(IntPtr device);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceStartSession(IntPtr device);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceStopSession(IntPtr device);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceStartService(IntPtr device, IntPtr service_name, out IntPtr handle, IntPtr unknown);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AMDeviceCopyValue(IntPtr device, IntPtr domain, IntPtr key);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceNotificationSubscribe(DeviceNotificationCallback callback, uint unused1, uint unused2, uint unused3, out IntPtr am_device_notification_ptr);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceInstallApplication(IntPtr sd_ip, IntPtr path, IntPtr option, InstallAppCallback callback, IntPtr unknown1);

        [DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceUninstallApplication(IntPtr sd_ip, IntPtr bundleIdentifier, IntPtr option, IntPtr unknown0, IntPtr unknown1);
    }
}

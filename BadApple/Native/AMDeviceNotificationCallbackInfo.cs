using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BadApple.Native
{

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct AMDeviceNotificationCallbackInfo
    {
        IntPtr dev_ptr;
        public NotificationMessage msg;
        public IntPtr Device
        {
            get
            {
                return this.dev_ptr;
            }
        }

        public AMDevice amdev
        {
            get
            {
                return (AMDevice)Marshal.PtrToStructure(dev_ptr, typeof(AMDevice));
            }
        }
    }
}


using BadApple.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BadApple
{
    public static class DeviceMonitor
    {
        static DeviceMonitor()
        {
            IntPtr ptr;

            var dnc = new DeviceNotificationCallback(DeviceMonitor.NotifyCallback);
            //this.drn1 = new DeviceRestoreNotificationCallback(this.DfuConnectCallback);
            //this.drn2 = new DeviceRestoreNotificationCallback(this.RecoveryConnectCallback);
            //this.drn3 = new DeviceRestoreNotificationCallback(this.DfuDisconnectCallback);
            //this.drn4 = new DeviceRestoreNotificationCallback(this.RecoveryDisconnectCallback);
            int num = iTunesMobileDevice.AMDeviceNotificationSubscribe(dnc, 0, 0, 0, out ptr);
            if (num != 0)
                throw new Exception("AMDeviceNotificationSubscribe failed with error " + num);

        }

        static void NotifyCallback(ref AMDeviceNotificationCallbackInfo info)
        {
            switch (info.msg)
            {
                case NotificationMessage.Connected:
                    Debug.WriteLine("连接");
                    Debug.WriteLine(info.Device);
                    OnConnected(new DeviceEventArgs() { Device = info.Device });
                    break;
                case NotificationMessage.Disconnected:
                    Debug.WriteLine("断开");
                    Debug.WriteLine(info.Device);
                    OnDisconnected(new DeviceEventArgs() { Device = info.Device });
                    break;
                case NotificationMessage.Unknown:
                    Debug.WriteLine("断开");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        static public event EventHandler<DeviceEventArgs> Connected;
        static public event EventHandler<DeviceEventArgs> Disconnected;

        static void OnConnected(DeviceEventArgs e)
        {
            var temp = Connected;
            if (temp != null)
            {
                temp(null, e);
            }
        }

        static void OnDisconnected(DeviceEventArgs e)
        {
            var temp = Disconnected;
            if (temp != null)
            {
                temp(null, e);
            }
        }
    }

    public class DeviceEventArgs : EventArgs
    {
        public IntPtr Device { get; set; }
    }
}

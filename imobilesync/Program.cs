using BadApple;
using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace imobilesync
{
    class Program
    {
        static IntPtr dev;

        static void Main(string[] args)
        {
            DeviceMonitor.Connected += DeviceMonitor_Connected;
            DeviceMonitor.Disconnected += DeviceMonitor_Disconnected;

            Console.WriteLine("等待设备连接。。。");
            Console.ReadLine();

            Debug.Assert(dev != IntPtr.Zero);

            Lockdown lockdown = new Lockdown(dev);

            var name = lockdown.GetValue("DeviceName");
            Console.WriteLine(name);

            var sd_sync = lockdown.StartService("com.apple.mobilesync");

            //var dl = new DeviceLinkService(sd_sync);
            //dl.device_link_service_version_exchange(300, 100);

            //MobileSync sync = new MobileSync(sd_sync);

            eti_sync eti = new eti_sync(sd_sync);

            eti.eti_sync_start_sync();
            var c = eti.eti_sync_get_contacts();

            var contacts = new Dictionary<string, EtiContact>();

            contacts.Add("6", new EtiContact()
            {
                first_name = "pizixie",
                first_name_yomi = "yomi",
                middle_name = " "
            });

            eti.eti_sync_send_contacts(contacts);
        }


        static void DeviceMonitor_Disconnected(object sender, DeviceEventArgs e)
        {
            Console.WriteLine("设备已断开");
        }

        static void DeviceMonitor_Connected(object sender, DeviceEventArgs e)
        {
            dev = e.Device;
            Console.WriteLine("设备已连接");
        }
    }
}

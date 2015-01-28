using BadApple;
using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ispringboard
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

            var sd_sbs = lockdown.StartService("com.apple.springboardservices");

            var sbs = new SpringBoard(sd_sbs);

            //桌面状态
            var plist_state = sbs.GetIconState();

            var file_state = System.IO.Path.GetTempFileName() + ".xml";
            plist_state.Save(file_state, PListFormat.Xml);
            Process.Start("IEXPLORE.EXE", file_state);

            //PNG
            var plist_png = sbs.GetIconPNGData("com.baidu.map");

            var file_png = System.IO.Path.GetTempFileName() + ".xml";
            plist_png.Save(file_png, PListFormat.Xml);
            Process.Start("IEXPLORE.EXE", file_png);

            Console.ReadLine();
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

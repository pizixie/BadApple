using BadApple;
using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var state = sbs.GetIconState();
            
            var state_xml = System.IO.Path.GetTempFileName() + ".xml";
            state.WriteXml(System.Xml.XmlWriter.Create(state_xml));
            Process.Start("IEXPLORE.EXE", state_xml);

            //ICON
            var png = sbs.GetIconPNGData("com.baidu.map");

            var png_xml = System.IO.Path.GetTempFileName() + ".xml";
            png.WriteXml(System.Xml.XmlWriter.Create(png_xml));
            Process.Start("IEXPLORE.EXE", png_xml);

            //墙纸
            var wallpaper = sbs.GetHomeScreenWallpaperPNGData();

            var file_wallpaper = System.IO.Path.GetTempFileName() + ".png";

            using (var stream_wallpaper = new FileStream(file_wallpaper, FileMode.Create))
            {
                var wallpaper_png = wallpaper["pngData"] as PListData;
                
                stream_wallpaper.Write(wallpaper_png.Value, 0, wallpaper_png.Value.Length);
            }
            Process.Start("IEXPLORE.EXE", file_wallpaper);

            //界面方向
            var s = sbs.GetInterfaceOrientation();
            Console.WriteLine(s);

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

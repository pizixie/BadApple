using BadApple;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BadApple.Native;
using CoreFoundation;

namespace ideviceinstaller
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

            var sd_ip = lockdown.StartService("com.apple.mobile.installation_proxy");

            var client_ip = new InstallationProxy(sd_ip);

            //应用列表
            var result = client_ip.GetAppList();

            var temp_file = System.IO.Path.GetTempFileName() + ".xml";
            result.Save(temp_file, PList.PListFormat.Xml);

            Process.Start("IEXPLORE.EXE", temp_file);

            //卸载
            var id = "com.fengbangshou.fbs";
            client_ip.UninstallApplication(id);

            //安装
            client_ip.InstallApplication("测试应用.ipa", new InstallAppCallback(Callback));
        }

        static void Callback(IntPtr ptr)
        {
            CFDictionary dic = new CFDictionary(ptr);

            Console.WriteLine(dic.ToString());


            //CFType v1 = dic.GetValue("Status");
            //CFType v2 = dic.GetValue("PercentComplete");
            //CFType v3 = dic.GetValue("Error");

            //var status = v1.ToString();
            //var percent = v2.ToString();

            //var error = v3.ToString();
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

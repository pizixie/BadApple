using BadApple.Native;
using CoreFoundation;
using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BadApple
{
    public class InstallationProxy
    {
        IntPtr sd;
        object sync = new object();

        public InstallationProxy(IntPtr sd)
        {
            this.sd = sd;
        }

        public PListRoot GetAppList()
        {
            lock (sync)
            {
                PListRoot send_plist = CreateBrowsePlist();

                PropertyListService.Send(sd, send_plist);

                PListArray array = new PListArray();

                bool browsing;
                do
                {
                    browsing = false;

                    PListRoot recv_plist = PropertyListService.Receive(sd);
                    var dic = recv_plist.Root as PListDict;

                    var status = (dic["Status"] as PListString).Value;

                    if (status == "BrowsingApplications")
                    {
                        browsing = true;

                        var amount = dic["CurrentAmount"] as PListInteger;
                        var current_list = dic["CurrentList"] as PListArray;

                        array.AddRange(current_list);
                    }
                    else
                    {
                        //status == "Complete"
                        Debug.WriteLine("Browse 完成");
                    }
                } while (browsing);

                return new PListRoot() { Root = array };
            }
        }

        private PListRoot CreateBrowsePlist()
        {
            PListArray attr = new PListArray();
            attr.Add(new PListString("CFBundleIdentifier"));
            //attr.Add(new PListString("DynamicDiskUsage"));应用过大会导致installd计算严重耗时
            attr.Add(new PListString("StaticDiskUsage"));
            attr.Add(new PListString("CFBundleDisplayName"));
            attr.Add(new PListString("CFBundleVersion"));
            attr.Add(new PListString("ApplicationType"));

            PListDict options = new PListDict();
            options.Add("ReturnAttributes", attr);
            options.Add("ApplicationType", new PListString("Any"));//User|System|Any

            PListDict root = new PListDict();
            root.Add("ClientOptions", options);
            root.Add("Command", new PListString("Browse"));

            PListRoot plist = new PListRoot();
            plist.Format = PListFormat.Xml;
            plist.Root = root;

            return plist;
        }

        public int InstallApplication(string path, InstallAppCallback callback)
        {
            lock (sync)
            {
                CFString cf_path = new CFString(path);

                var i = iTunesMobileDevice.AMDeviceInstallApplication(sd, cf_path.Handle, IntPtr.Zero, callback, IntPtr.Zero);

                return i;
            }
        }

        public int UpgradeApplication(string path, InstallAppCallback callback)
        {
            throw new NotImplementedException();
        }

        public int UninstallApplication(string id)
        {
            lock (sync)
            {
                CFString cf_id = new CFString(id);

                var i = iTunesMobileDevice.AMDeviceUninstallApplication(sd, cf_id.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                return i;
            }
        }

        public int TransferApplication(string path, InstallAppCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}

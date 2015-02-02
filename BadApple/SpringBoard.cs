using PList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class SpringBoard
    {
        IntPtr sd;
        object sync = new object();

        public SpringBoard(IntPtr sd)
        {
            this.sd = sd;
        }

        public PListRoot GetIconState()
        {
            lock (sync)
            {
                PListDict root = new PListDict();
                root.Add("command", new PListString("getIconState"));
                root.Add("formatVersion", new PListString("2"));

                PListRoot send_plist = new PListRoot();
                send_plist.Format = PListFormat.Xml;
                send_plist.Root = root;

                PropertyListService.Send(sd, send_plist);

                PListRoot recv_plist = PropertyListService.Receive(sd);

                return recv_plist;
            }
        }

        public void SetIconState()
        {
            throw new NotImplementedException();
        }

        public PListRoot GetIconPNGData(string bundle_id)
        {
            lock (sync)
            {
                PListDict root = new PListDict();
                root.Add("command", new PListString("getIconPNGData"));
                root.Add("bundleId", new PListString(bundle_id));

                PListRoot out_plist = new PListRoot();
                out_plist.Format = PListFormat.Xml;
                out_plist.Root = root;

                PropertyListService.Send(sd, out_plist);

                PListRoot in_plist = PropertyListService.Receive(sd);

                return in_plist;
            }
        }

        public InterfaceOrientation GetInterfaceOrientation()
        {
            lock (sync)
            {
                PListDict dict = new PListDict();
                dict.Add("command", new PListString("getInterfaceOrientation"));

                PListRoot send = new PListRoot();
                send.Format = PListFormat.Xml;
                send.Root = dict;

                PropertyListService.Send(sd, send);

                var recv = PropertyListService.Receive(sd);

                var content = recv.Root as PListDict;
                var v = content["interfaceOrientation"] as PListInteger;

                return (InterfaceOrientation)v.Value;
            }
        }

        public PListRoot GetHomeScreenWallpaperPNGData()
        {
            //throw new NotImplementedException();
            lock (sync)
            {
                PListDict dict = new PListDict();
                dict.Add("command", new PListString("getHomeScreenWallpaperPNGData"));

                PListRoot send = new PListRoot();
                send.Format = PListFormat.Xml;
                send.Root = dict;

                PropertyListService.Send(sd, send);

                PListRoot recv = PropertyListService.Receive(sd);

                return recv;
            }
        }

        public enum InterfaceOrientation
        {
            UNKNOWN = 0,
            PORTRAIT = 1,
            PORTRAIT_UPSIDE_DOWN = 2,
            LANDSCAPE_RIGHT = 3,
            LANDSCAPE_LEFT = 4
        }
    }

}

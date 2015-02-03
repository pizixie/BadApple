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

        public PListDict GetIconState()
        {
            lock (sync)
            {
                PListDict root = new PListDict();
                root.Add("command", new PListString("getIconState"));
                root.Add("formatVersion", new PListString("2"));
                
                PropertyListService.Send(sd, root);

                var recv_plist = PropertyListService.Receive(sd);

                return recv_plist as PListDict;
            }
        }

        public void SetIconState()
        {
            throw new NotImplementedException();
        }

        public PListDict GetIconPNGData(string bundle_id)
        {
            lock (sync)
            {
                PListDict root = new PListDict();
                root.Add("command", new PListString("getIconPNGData"));
                root.Add("bundleId", new PListString(bundle_id));

                PropertyListService.Send(sd, root);

                var recv = PropertyListService.Receive(sd);

                return recv as PListDict;
            }
        }

        public InterfaceOrientation GetInterfaceOrientation()
        {
            lock (sync)
            {
                PListDict dict = new PListDict();
                dict.Add("command", new PListString("getInterfaceOrientation"));

                PropertyListService.Send(sd, dict);

                var recv = PropertyListService.Receive(sd) as PListDict;

                var v = recv["interfaceOrientation"] as PListInteger;

                return (InterfaceOrientation)v.Value;
            }
        }

        public PListDict GetHomeScreenWallpaperPNGData()
        {
            //throw new NotImplementedException();
            lock (sync)
            {
                PListDict dict = new PListDict();
                dict.Add("command", new PListString("getHomeScreenWallpaperPNGData"));

                PropertyListService.Send(sd, dict);

                var recv = PropertyListService.Receive(sd);

                return recv as PListDict;
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

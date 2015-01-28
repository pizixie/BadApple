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
        IntPtr fd;
        object sync;

        public SpringBoard(IntPtr fd)
        {
            this.fd = fd;
        }

        public byte[] DownloadIcon(string bundle_id)
        {
            lock (sync)
            {
                PListDict root = new PListDict();
                root.Add("command", new PListString("getIconPNGData"));
                root.Add("bundleId", new PListString(bundle_id));

                PListRoot out_plist = new PListRoot();
                out_plist.Format = PListFormat.Xml;
                out_plist.Root = root;

                PropertyListService.Send(fd, out_plist);

                PListRoot in_plist = PropertyListService.Receive(fd);

                var dict = in_plist.Root as PListDict;
                var png_data = dict["pngData"] as PListData;

                return png_data.Value;
            }
        }

        public List<IconState> GetIconStates()
        {
            lock (sync)
            {
                PListDict root = new PListDict();
                root.Add("command", new PListString("getIconState"));
                root.Add("formatVersion", new PListString("2"));

                PListRoot send_plist = new PListRoot();
                send_plist.Format = PListFormat.Xml;
                send_plist.Root = root;

                PropertyListService.Send(fd, send_plist);

                PListRoot recv_plist = PropertyListService.Receive(fd);

                var states = Plist2IconStateList(recv_plist);

                return states;
            }
        }

        private List<IconState> Plist2IconStateList(PListRoot plist)
        {
            List<IconState> list = new List<IconState>();

            PListArray root = plist.Root as PListArray;
            //root[0]    -----底部四项应用
            //root[1]    -----首页
            //root[2]    -----应用第一页
            //root[3]    -----应用第二页

            //底部四项应用
            RecGetIconState(list, root);

            return list;
        }

        private void RecGetIconState(List<IconState> list, IPListElement node)
        {
            if (node is PListArray)
            {
                foreach (var child in node as PListArray)
                {
                    RecGetIconState(list, child);
                }
            }
            else
            {
                var dict = node as PListDict;

                IPListElement list_type;
                if (dict.TryGetValue("listType", out list_type))
                {
                    var icon_list = dict["iconLists"] as PListArray;

                    RecGetIconState(list, icon_list);
                }
                else
                {
                    list.Add(PlistDict2IconState(dict));
                }
            }
        }

        private IconState PlistDict2IconState(PListDict dict)
        {
            IconState icon = new IconState();
            icon.BundleId = GetPlistStringValue(dict, "bundleIdentifier");
            icon.DisplayId = GetPlistStringValue(dict, "displayIdentifier");
            icon.DisplayName = GetPlistStringValue(dict, "displayName");
            icon.IconModDate = GetPlistDateValue(dict, "iconModDate");
            icon.BundleVersion = GetPlistStringValue(dict, "bundleVersion");

            return icon;
        }

        private string GetPlistStringValue(PListDict dic, string key)
        {
            IPListElement v;
            if (dic.TryGetValue(key, out v))
            {
                var temp = v as PListString;
                return temp.Value;
            }

            return null;
        }

        private DateTime? GetPlistDateValue(PListDict dic, string key)
        {
            IPListElement ele;
            if (dic.TryGetValue(key, out ele))
            {
                var v = ele as PListDate;
                return v.Value;
            }

            return null;
        }
    }

    public class IconState
    {
        public string DisplayId { get; set; }
        public string DisplayName { get; set; }
        public DateTime? IconModDate { get; set; }
        public string BundleVersion { get; set; }
        public string BundleId { get; set; }
    }
}

using PList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadApple
{
    public static class PListExtensions
    {
        public static string ToXmlString(this IPListElement plist)
        {
            StringBuilder builder = new StringBuilder();

            var writer = System.Xml.XmlWriter.Create(builder);

            plist.WriteXml(writer);

            writer.Flush();

            return builder.ToString();
        }
    }
}

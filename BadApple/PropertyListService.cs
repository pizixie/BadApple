using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PList;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;

namespace BadApple
{
    public static class PropertyListService
    {
        public static void Send(IntPtr fd, IPListElement plist)
        {
            PListRoot root = new PListRoot();
            root.Root = plist;

            byte[] buffer;

            using (MemoryStream stream = new MemoryStream())
            {
                root.Save(stream, PListFormat.Xml);
                buffer = stream.ToArray();
            }

            var be_length = IPAddress.HostToNetworkOrder(buffer.Length);
            var be_buffer = BitConverter.GetBytes(be_length);

            int sent;

            sent = send(fd, be_buffer, 4, 0);
            if (sent != 4)
                throw new PlistServiceException("发送数据边界失败");

            sent = send(fd, buffer, buffer.Length, 0);
            if (sent != buffer.Length)
                throw new PlistServiceException("发送数据失败");
        }

        public static IPListElement Receive(IntPtr fd)
        {
            int bytes = 0;
            IntPtr p_length = IntPtr.Zero;
            IntPtr p_content = IntPtr.Zero;

            try
            {
                //读取边界
                p_length = Marshal.AllocHGlobal(4);
                bytes = recv(fd, p_length, 4, 0);

                if (bytes != 4)
                    throw new PlistServiceException("接收数据边界失败");

                var be_length = Marshal.ReadInt32(p_length);
                var length = IPAddress.NetworkToHostOrder(be_length);

                if (length > (1 << 24))
                    throw new PlistServiceException("接收数据长度过大");

                //读取内容
                p_content = Marshal.AllocHGlobal(length);

                int offset = 0;
                while (offset < length)
                {
                    bytes = recv(fd, new IntPtr((long)p_content + offset), length - offset, 0);

                    if (bytes <= 0)
                        throw new PlistServiceException("接收数据失败");

                    offset += bytes;
                }

                byte[] content = new byte[length];
                Marshal.Copy(p_content, content, 0, length);

                using (MemoryStream stream = new MemoryStream(content))
                {
                    PListRoot plist = PListRoot.Load(stream);
                    return plist.Root;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(p_length);
                Marshal.FreeHGlobal(p_content);
            }

        }

        [DllImport("ws2_32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int send(IntPtr s, byte[] buffer, int len, int flags);

        [DllImport("ws2_32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int recv(IntPtr s, IntPtr buf, int len, int flags);
    }

    public class PlistServiceException : Exception
    {
        public PlistServiceException(string msg)
            : base(msg)
        {

        }
    }
}

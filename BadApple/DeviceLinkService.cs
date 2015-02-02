using PList;
using System;
using System.Collections.Generic;
using System.Text;

namespace BadApple
{
    public class DeviceLinkService
    {
        IntPtr sd;

        public DeviceLinkService(IntPtr sd)
        {
            this.sd = sd;
        }

        public PListRoot VersionExchange(long major,long minor)
        {
            PListRoot recv;
            PListString msg;

            /* receive DLMessageVersionExchange from device */
            recv = PropertyListService.Receive(sd);

            /* get major and minor version number */
            var array = recv.Root as PListArray;

            msg = array[0] as PListString;
            var value_major = array[1] as PListInteger;
            var value_minor = array[2] as PListInteger;

            //todo:validate major and minor

            /* version is ok, send reply */
            var relay_array = new PListArray();
            relay_array.Add(new PListString("DLMessageVersionExchange"));
            relay_array.Add(new PListString("DLVersionsOk"));
            relay_array.Add(new PListInteger(minor));

            var relay_plist = new PListRoot();
            relay_plist.Root = relay_array;

            PropertyListService.Send(sd, relay_plist);

            /* receive DeviceReady message */
            recv = PropertyListService.Receive(sd);

            msg = array[0] as PListString;

            return recv;
        }
    }

    [Serializable]
    public class DeviceLinkServiceException : Exception
    {
        public DeviceLinkServiceException() { }
        public DeviceLinkServiceException(string message) : base(message) { }
        public DeviceLinkServiceException(string message, Exception inner) : base(message, inner) { }
        protected DeviceLinkServiceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

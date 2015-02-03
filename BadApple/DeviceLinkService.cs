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

        public void device_link_service_version_exchange(long major, long minor)
        {
            PListArray array;
            PListString msg;

            /* receive DLMessageVersionExchange from device */
            array = PropertyListService.Receive(sd) as PListArray;

            msg = array[0] as PListString;

            if (msg.Value != "DLMessageVersionExchange")
                throw new DeviceLinkServiceException("Did not receive DLMessageVersionExchange from device!");

            /* get major and minor version number */
            if (array.Count < 3)
                throw new DeviceLinkServiceException("DLMessageVersionExchange has unexpected format!");

            var value_major = (array[1] as PListInteger).Value;
            var value_minor = (array[2] as PListInteger).Value;

            if (value_major > major)
            {
                throw new DeviceLinkServiceException(
                    String.Format("Version mismatch: device=({0},{1}) > expected=({2},{3})",
                    value_major, value_minor, major, minor));
            }
            else if (value_major == major && value_minor > minor)
            {
                throw new DeviceLinkServiceException(
                    String.Format("WARNING: Version mismatch: device=(%lld,%lld) > expected=(%lld,%lld)",
                    value_major, value_minor, major, minor));
            }

            /* version is ok, send reply */
            var relay_array = new PListArray();
            relay_array.Add(new PListString("DLMessageVersionExchange"));
            relay_array.Add(new PListString("DLVersionsOk"));
            relay_array.Add(new PListInteger(minor));

            PropertyListService.Send(sd, relay_array);

            /* receive DeviceReady message */
            var ready_array = PropertyListService.Receive(sd) as PListArray;

            msg = ready_array[0] as PListString;
            if (msg.Value != "DLMessageDeviceReady")
                throw new DeviceLinkServiceException("Did not get DLMessageDeviceReady!");
        }

        public void device_link_service_disconnect(string msg)
        {
            PListArray array = new PListArray();
            array.Add(new PListString("DLMessageDisconnect"));

            if (msg != null)
                array.Add(new PListString(msg));
            else
                array.Add(new PListString("___EmptyParameterString___"));

            PropertyListService.Send(this.sd, array);
        }

        public void device_link_service_send_ping(string msg)
        {
            PListArray array = new PListArray();
            array.Add(new PListString("DLMessagePing"));
            array.Add(new PListString(msg));

            PropertyListService.Send(this.sd, array);
        }

        public void device_link_service_send_process_message(PListDict msg)
        {
            PListArray array = new PListArray();
            array.Add(new PListString("DLMessageProcessMessage"));
            array.Add(msg);

            PropertyListService.Send(this.sd, array);
        }

        public IPListElement device_link_service_receive_process_message()
        {
            var array = PropertyListService.Receive(this.sd) as PListArray;

            if (array.Count != 2)
                throw new DeviceLinkServiceException("Malformed plist received for DLMessageProcessMessage");

            var msg = array[0] as PListString;
            if (msg.Value != "DLMessageProcessMessage")
                throw new DeviceLinkServiceException("Did not receive DLMessageProcessMessage as expected!");

            return array[1];
        }

        public void device_link_service_send(IPListElement plist)
        {
            PropertyListService.Send(this.sd, plist);
        }

        public IPListElement device_link_service_receive()
        {
            return PropertyListService.Receive(this.sd);
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

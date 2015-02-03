using PList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class MobileSync
    {
        DeviceLinkService dls;

        string data_class;
        mobilesync_sync_direction_t direction;

        object sync = new object();

        public MobileSync(IntPtr sd)
        {
            this.dls = new DeviceLinkService(sd);
            this.direction = mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_DEVICE_TO_COMPUTER;
            this.data_class = null;

            this.dls.device_link_service_version_exchange(300, 100);
        }

        public IPListElement mobilesync_receive()
        {
            return this.dls.device_link_service_receive();
        }

        public void mobilesync_send(IPListElement plist)
        {
            this.dls.device_link_service_send(plist);
        }

        public void mobilesync_start(string data_class, mobilesync_anchors_t anchors, long computer_data_class_version, out mobilesync_sync_type_t sync_type, out long device_data_class_version, out string error_description)
        {
            PListArray msg;

            error_description = null;

            msg = new PListArray();
            msg.Add(new PListString("SDMessageSyncDataClassWithDevice"));
            msg.Add(new PListString(data_class));

            if (anchors.device_anchor != null)
                msg.Add(new PListString(anchors.device_anchor));
            else
                msg.Add(new PListString("---"));

            msg.Add(new PListString(anchors.computer_anchor));
            msg.Add(new PListInteger(computer_data_class_version));
            msg.Add(new PListString("___EmptyParameterString___"));

            mobilesync_send(msg);

            msg = mobilesync_receive() as PListArray;

            var response_type = msg[0] as PListString;
            if (response_type.Value == "SDMessageRefuseToSyncDataClassWithComputer")
            {
                var error = msg[2] as PListString;

                error_description = error.Value;

                throw new MobileSyncException(String.Format("Device refused sync: {0}", error.Value));
            }

            if (response_type.Value == "SDMessageCancelSession")
            {
                var error = msg[2] as PListString;

                error_description = error.Value;

                throw new MobileSyncException(String.Format("Device cancelled: {0}", error.Value));
            }

            var sync_type_node = msg[4] as PListString;
            if (sync_type_node == null)
                throw new MobileSyncException();

            if (sync_type_node.Value == "SDSyncTypeFast")
            {
                sync_type = mobilesync_sync_type_t.MOBILESYNC_SYNC_TYPE_FAST;
            }
            else if (sync_type_node.Value == "SDSyncTypeSlow")
            {
                sync_type = mobilesync_sync_type_t.MOBILESYNC_SYNC_TYPE_SLOW;
            }
            else
            {
                throw new MobileSyncException();
            }

            var device_data_class_version_node = msg[5] as PListInteger;
            device_data_class_version = device_data_class_version_node.Value;
        }
    }

    public enum mobilesync_sync_direction_t
    {
        MOBILESYNC_SYNC_DIR_DEVICE_TO_COMPUTER,
        MOBILESYNC_SYNC_DIR_COMPUTER_TO_DEVICE
    }

    public struct mobilesync_anchors_t
    {
        public string device_anchor;
        public string computer_anchor;
    }

    public enum mobilesync_sync_type_t
    {
        MOBILESYNC_SYNC_TYPE_FAST, /**< Fast-sync requires that only the changes made since the last synchronization should be reported by the computer. */
        MOBILESYNC_SYNC_TYPE_SLOW, /**< Slow-sync requires that all data from the computer needs to be synchronized/sent. */
        MOBILESYNC_SYNC_TYPE_RESET /**< Reset-sync signals that the computer should send all data again. */
    }

    [Serializable]
    public class MobileSyncException : Exception
    {
        public MobileSyncException() { }
        public MobileSyncException(string message) : base(message) { }
        public MobileSyncException(string message, Exception inner) : base(message, inner) { }
        protected MobileSyncException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

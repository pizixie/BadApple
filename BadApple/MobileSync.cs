using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BadApple
{
    public class MobileSync
    {
        DeviceLinkService dls;

        string data_class;
        mobilesync_sync_direction_t direction;

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

            this.data_class = data_class;
            this.direction = mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_DEVICE_TO_COMPUTER;
        }

        public void mobilesync_finish()
        {
            PListArray msg;
            PListString response_type_node;
            string response_type;

            msg = new PListArray();
            msg.Add(new PListString("SDMessageFinishSessionOnDevice"));
            msg.Add(new PListString(this.data_class));

            mobilesync_send(msg);

            msg = mobilesync_receive() as PListArray;

            response_type_node = msg[0] as PListString;

            response_type = response_type_node.Value;

            if (response_type != "SDMessageDeviceFinishedSession")
                throw new MobileSyncException();
        }

        public void mobilesync_get_records(string operation)
        {
            PListArray msg = new PListArray();

            msg.Add(new PListString(operation));
            msg.Add(new PListString(this.data_class));

            mobilesync_send(msg);
        }

        public void mobilesync_get_all_records_from_device()
        {
            mobilesync_get_records("SDMessageGetAllRecordsFromDevice");
        }

        public void mobilesync_get_changes_from_device()
        {
            mobilesync_get_records("SDMessageGetChangesFromDevice");
        }

        public void mobilesync_receive_changes(out IPListElement entities, out long is_last_record, out IPListElement actions)
        {
            PListArray msg;
            PListString response_type_node;
            string response_type;
            long has_more_changes;
            IPListElement actions_node;

            msg = mobilesync_receive() as PListArray;
            response_type_node = msg[0] as PListString;
            response_type = response_type_node.Value;

            if (response_type == "SDMessageCancelSession")
            {
                var reason = (msg[2] as PListString).Value;
                throw new MobileSyncException(String.Format("Device cancelled: {0}", reason));
            }

            entities = msg[2];

            has_more_changes = (msg[3] as PListBool).Value ? 1 : 0;

            is_last_record = has_more_changes > 0 ? 0 : 1;

            actions_node = msg[4];

            if (actions_node is PListDict)
            {
                actions = actions_node;
            }
            else
            {
                actions = null;
            }
        }

        public void mobilesync_clear_all_records_on_device()
        {
            PListArray msg;
            PListString response_type_node;
            string response_type;

            msg = new PListArray();
            msg.Add(new PListString("SDMessageClearAllRecordsOnDevice"));
            msg.Add(new PListString(this.data_class));
            msg.Add(new PListString(EMPTY_PARAMETER_STRING));

            mobilesync_send(msg);

            msg = mobilesync_receive() as PListArray;

            response_type_node = msg[0] as PListString;
            response_type = response_type_node.Value;

            if (response_type == "SDMessageCancelSession")
            {
                string reason = (msg[2] as PListString).Value;

                throw new MobileSyncException(String.Format("Device cancelled: {0}", reason));
            }

            if (response_type != "SDMessageDeviceWillClearAllRecords")
                throw new MobileSyncException();

        }

        public void mobilesync_acknowledge_changes_from_device()
        {
            PListArray msg = new PListArray();
            msg.Add(new PListString("SDMessageAcknowledgeChangesFromDevice"));
            msg.Add(new PListString(this.data_class));

            mobilesync_send(msg);
        }

        public PListArray create_process_changes_message(string data_class, IPListElement entities, long more_changes, IPListElement actions)
        {
            PListArray msg = new PListArray();
            msg.Add(new PListString("SDMessageProcessChanges"));
            msg.Add(new PListString(data_class));
            msg.Add(entities);
            msg.Add(new PListBool(more_changes != 0));

            if (actions != null)
                msg.Add(actions);
            else
                msg.Add(new PListString(EMPTY_PARAMETER_STRING));

            return msg;
        }

        public void mobilesync_ready_to_send_changes_from_computer()
        {
            if (this.direction != mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_DEVICE_TO_COMPUTER)
                throw new MobileSyncException("MOBILESYNC_E_WRONG_DIRECTION");

            PListArray msg;
            PListString response_type_node;
            string response_type;

            msg = mobilesync_receive() as PListArray;

            response_type_node = msg[0] as PListString;
            response_type = response_type_node.Value;

            if (response_type == "SDMessageCancelSession")
            {
                string reason = (msg[2] as PListString).Value;

                throw new MobileSyncException(String.Format("Device cancelled: {0}", reason));
            }

            if (response_type != "SDMessageDeviceReadyToReceiveChanges")
            {
                throw new MobileSyncException("MOBILESYNC_E_NOT_READY");
            }

            this.dls.device_link_service_send_ping("Preparing to get changes for device");

            this.direction = mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_COMPUTER_TO_DEVICE;
        }

        public void mobilesync_send_changes(PListDict entities, long is_last_record, IPListElement actions)
        {
            if (this.direction != mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_COMPUTER_TO_DEVICE)
                throw new MobileSyncException("MOBILESYNC_E_WRONG_DIRECTION");

            var msg = create_process_changes_message(this.data_class, entities, (is_last_record > 0) ? 0 : 1, actions);

            mobilesync_send(msg);
        }

        public void mobilesync_remap_identifiers(out IPListElement mapping)
        {
            if (this.direction == mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_DEVICE_TO_COMPUTER)
                throw new MobileSyncException("MOBILESYNC_E_WRONG_DIRECTION");

            PListArray msg;
            PListString response_type_node;
            string response_type;

            msg = mobilesync_receive() as PListArray;

            response_type_node = msg[0] as PListString;
            response_type = response_type_node.Value;

            if (response_type == "SDMessageCancelSession")
            {
                string reason = (msg[2] as PListString).Value;

                throw new MobileSyncException(String.Format("Device cancelled: {0}", reason));
            }

            if (response_type != "SDMessageRemapRecordIdentifiers")
                throw new MobileSyncException("MOBILESYNC_E_PLIST_ERROR");

            var map = msg[2];
            if (map is PListDict)
                mapping = map;
            else
                mapping = null;
        }

        public void mobilesync_cancel(string reason)
        {
            PListArray msg = new PListArray();
            msg.Add(new PListString("SDMessageCancelSession"));
            msg.Add(new PListString(this.data_class));
            msg.Add(new PListString(reason));

            mobilesync_send(msg);

            this.data_class = null;
            this.direction = mobilesync_sync_direction_t.MOBILESYNC_SYNC_DIR_DEVICE_TO_COMPUTER;
        }

        public mobilesync_anchors_t mobilesync_anchors_new(string device_anchor, string computer_anchor)
        {
            mobilesync_anchors_t anchors = new mobilesync_anchors_t();

            anchors.device_anchor = device_anchor;
            anchors.computer_anchor = computer_anchor;

            return anchors;
        }

        PListDict mobilesync_actions_new()
        {
            return new PListDict();
        }

        public void mobilesync_actions_add(PListDict actions, string key, string[] entity_names, bool link_records)
        {
            if (key == "SyncDeviceLinkEntityNamesKey")
            {
                PListArray array = new PListArray();

                for (int i = 0; i < entity_names.Length; i++)
                {
                    array.Add(new PListString(entity_names[i]));
                }

                actions.Add(key, array);
            }
            else if(key == "SyncDeviceLinkAllRecordsOfPulledEntityTypeSentKey")
            {
                actions.Add(key, new PListBool(link_records));
            }
        }

        public void mobilesync_actions_free(IPListElement actions)
        {
            throw new NotImplementedException();
        }

        const string EMPTY_PARAMETER_STRING = "___EmptyParameterString___";
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

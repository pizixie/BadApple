using BadApple;
using PList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace eds_to_idevice_eds_to_idevice
{
    public class eti_sync
    {
        MobileSync sync;

        public eti_sync(IntPtr sd)
        {
            sync = new MobileSync(sd);
        }

        public void eti_sync_start_sync()
        {
            DateTime cur_time;
            string cur_time_str;
            string host_anchor;
            string error_description;

            mobilesync_sync_type_t sync_type;
            long device_data_class_version;
            mobilesync_anchors_t anchors;

            cur_time = DateTime.Now;
            cur_time_str = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
            host_anchor = String.Format("eti-{0}", cur_time_str);
            anchors = sync.mobilesync_anchors_new(null, host_anchor);

            sync.mobilesync_start("com.apple.Contacts", anchors, EDI_CLASS_STORAGE_VERSION, out sync_type, out device_data_class_version, out error_description);
        }

        //GHashTable* eti_sync_get_contacts(EtiSync* sync, GError** error)
        public List<object> eti_sync_get_contacts()
        {
            IPListElement entities;
            IPListElement actions;
            long is_last;
            var contacts = new List<Object>();

            sync.mobilesync_get_all_records_from_device();

            do
            {
                sync.mobilesync_receive_changes(out entities, out is_last, out actions);

                var xml_string = entities.ToXmlString();

                PListRoot r = new PListRoot();
                r.Format = PListFormat.Xml;
                r.Root = entities;

                var xml = System.IO.Path.GetTempFileName() + ".xml";
                r.Save(xml);

                Process.Start("IEXPLORE.EXE", xml);

                sync.mobilesync_acknowledge_changes_from_device();

                Debug.WriteLine(entities);

                //    if (!eti_contact_plist_parser_parse(parser, entities, error))
                //        break;

            } while (is_last == 0);

            //contacts = eti_contact_plist_parser_get_contacts(parser);

            return contacts;
        }

        //static plist_t send_one(EtiSync* sync, plist_t entities, gboolean is_last, GError** error)
        IPListElement send_one(PListDict entities, bool is_last)
        {
            IPListElement remapped_identifiers;

            sync.mobilesync_send_changes(entities, is_last ? 1 : 0, null);

            sync.mobilesync_remap_identifiers(out remapped_identifiers);

            return remapped_identifiers;
        }

        //void eti_sync_send_contacts(EtiSync* sync, GHashTable* contacts, GError** error)
        public void eti_sync_send_contacts(Dictionary<string,EtiContact> contacts)
        {
            PListDict main_entities;
            IPListElement remapped_uids;
            List<PListDict> plists;
            
            sync.mobilesync_ready_to_send_changes_from_computer();

            main_entities = eti_contact_plist_builder.eti_contact_plist_builder_build_main(contacts);

            remapped_uids = send_one(main_entities, false);

            //plists = eti_contact_plist_builder_build_others(contacts, remapped_uids);

            plists = new List<PListDict>();

            foreach (var item in plists)
            {
                remapped_uids = send_one(item, false);
                //最后一个
                //remapped_uids = send_one(item, true);
            }
        }

        //void eti_sync_wipe_all_contacts(EtiSync* sync, GError** error)
        public void eti_sync_wipe_all_contacts()
        {
            sync.mobilesync_clear_all_records_on_device();
        }

        //void eti_sync_stop_sync(EtiSync* sync, GError** error)
        public void eti_sync_stop_sync()
        {
            sync.mobilesync_finish();
        }

        const long EDI_CLASS_STORAGE_VERSION = 106;
    }

    [Serializable]
    public class EtiSyncException : Exception
    {
        public EtiSyncException() { }
        public EtiSyncException(string message) : base(message) { }
        public EtiSyncException(string message, Exception inner) : base(message, inner) { }
        protected EtiSyncException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

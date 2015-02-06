using System;
using System.Collections.Generic;
using System.Text;
using BadApple;
using PList;
using System.Diagnostics;

namespace imobilesync
{
    class msyncclient
    {
        public static void mobilesync_get_all_contacts(MobileSync client)
        {
            PListArray array = new PListArray();
            array.Add(new PListString("SDMessageSyncDataClassWithDevice"));
            array.Add(new PListString("com.apple.Contacts"));
            array.Add(new PListString("---"));
            array.Add(new PListString("2009-01-09 18:03:58 +0100"));
            array.Add(new PListInteger(106));
            array.Add(new PListString("___EmptyParameterString___"));

            client.mobilesync_send(array);

            array = client.mobilesync_receive() as PListArray;

            array = new PListArray();
            array.Add(new PListString("SDMessageGetAllRecordsFromDevice"));
            array.Add(new PListString("com.apple.Contacts"));

            client.mobilesync_send(array);

            array = client.mobilesync_receive() as PListArray;

            PListString contact_node = array[1] as PListString;
            PListString switch_node = array[0] as PListString;

            while (contact_node.Value == "com.apple.Contacts"
                && switch_node.Value != "SDMessageDeviceReadyToReceiveChanges")
            {
                array = new PListArray();
                array.Add(new PListString("SDMessageAcknowledgeChangesFromDevice"));
                array.Add(new PListString("com.apple.Contacts"));

                client.mobilesync_send(array);

                array = client.mobilesync_receive() as PListArray;

                contact_node = array[1] as PListString;
                switch_node = array[0] as PListString;
            }

            array = new PListArray();
            array.Add(new PListString("DLMessagePing"));
            array.Add(new PListString("Preparing to get changes for device"));

            client.mobilesync_send(array);

            array = new PListArray();
            array.Add(new PListString("SDMessageProcessChanges"));
            array.Add(new PListString("com.apple.Contacts"));
            array.Add(create_new_contact());
            array.Add(new PListBool(false));

            PListDict dict = new PListDict();
            array.Add(dict);
            PListArray array2 = new PListArray();
            dict.Add("SyncDeviceLinkEntityNamesKey", array2);
            array2.Add(new PListString("com.apple.contacts.Contact"));
            array2.Add(new PListString("com.apple.contacts.Group"));

            dict.Add("SyncDeviceLinkAllRecordsOfPulledEntityTypeSentKey", new PListBool(true));

            ShowIPListElement(array);

            client.mobilesync_send(array);

            array = client.mobilesync_receive() as PListArray;


            PListArray finish = new PListArray();
            finish.Add(new PListString("SDMessageFinishSessionOnDevice"));
            finish.Add(new PListString("com.apple.Contacts"));

            client.mobilesync_send(finish);

            finish = client.mobilesync_receive() as PListArray;
        }

        public static void ShowIPListElement(IPListElement plist)
        {
            PListRoot r = new PListRoot();
            r.Format = PListFormat.Xml;
            r.Root = plist;

            var xml = System.IO.Path.GetTempPath() + ".xml";

            r.Save(xml);

            Process.Start("IEXPLORE.EXE", xml);
        }

        public static PListDict create_new_contact()
        {
            PListDict contact = new PListDict();
            PListDict value = new PListDict();

            contact.Add("{B3C33737-35A0-401F-A25E-00D427D733F7}", value);

            //value.Add("birthday", new PListDate(DateTime.Now));
            value.Add("com.apple.syncservices.RecordEntityName", new PListString("com.apple.contacts.Contact"));
            value.Add("display as company", new PListString("person"));
            value.Add("last name", new PListString("pizixie"));

            return contact;
        }
    }
}

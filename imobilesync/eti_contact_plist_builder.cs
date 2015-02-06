using PList;
using System;
using System.Collections.Generic;
using System.Text;

namespace imobilesync
{
    class eti_contact_plist_builder
    {
        public static PListDict eti_contact_plist_builder_build_main(Dictionary<string, EtiContact> contacts)
        {
            var main_plist = new PListDict();

            foreach (var item in contacts)
            {
                var uid = item.Key;
                var contact = item.Value;

                var main_info = new PListDict();
                DateTime birthday;
                byte[] image_data;
                int data_length;

                main_info.Add("com.apple.syncservices.RecordEntityName", new PListString("com.apple.contacts.Contact"));
                main_info.Add("first name", new PListString(contact.first_name));
                main_info.Add("first name yomi", new PListString(contact.first_name_yomi));
                main_info.Add("middle name", new PListString(contact.middle_name));
                //... lat name,nickname,title,suffix

                birthday = contact.birthday;

                main_info.Add("birthday", new PListDate(birthday));

                //main_info.Add("image", new PListData(contact.photo.image_data));

                main_plist.Add(uid, main_info);
            }

            return main_plist;
        }
    }
}

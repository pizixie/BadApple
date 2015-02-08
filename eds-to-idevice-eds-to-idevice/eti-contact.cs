using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eds_to_idevice_eds_to_idevice
{
    public class EtiContact
    {
        public EtiContactType type;
        public string first_name;
        public string first_name_yomi;
        public string middle_name;
        public string last_name;
        public string last_name_yomi;
        public string nickname;
        public string title;
        public string name_suffix;
        public string notes;
        public string company_name;
        public string department;
        public string job_title;
        public DateTime birthday;
        public List<EtiContactGenericMultifield> addresses;
        public List<EtiContactGenericMultifield> phone_numbers;
        public List<EtiContactGenericMultifield> emails;
        public List<EtiContactGenericMultifield> im_user_ids;
        public List<EtiContactGenericMultifield> urls;
        public List<EtiContactGenericMultifield> dates;
        public EtiContactPhoto photo;
    }

    public enum EtiContactType
    {
        ETI_CONTACT_TYPE_PERSON,
        ETI_CONTACT_TYPE_COMPANY
    }

    public class EtiContactPhoto
    {
        public byte[] image_data;
        public int data_length;
    }

    public class EtiContactAddress
    {
        public string street;
        public string postal_code;
        public string city;
        public string country;
        public string country_code;
    }

    public class EtiContactGenericMultifield
    {
        public string type;
        public string label;
        public object value;//string or address
    }

    public class EtiContactImUserId
    {
        public string service;
        public string user_id;
    }
}

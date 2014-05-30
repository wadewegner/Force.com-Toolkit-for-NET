using System.Collections.Generic;

namespace Salesforce.Common.Models
{
    public class UserInfo
    {
        public string id;
        public bool asserted_user;
        public string user_id;
        public string organization_id;
        public string username;
        public string nick_name;
        public string display_name;
        public string email;
        public bool email_verified;
        public string first_name;
        public string last_name;
        public Dictionary<string, string> status;
        public Dictionary<string, string> photos;
        public string addr_street;
        public string addr_city;
        public string addr_state;
        public string addr_country;
        public string addr_zip;
        public string mobile_phone;
        public bool mobile_phone_verified;
        public Dictionary<string, string> urls;
        public bool active;
        public string user_type;
        public string language;
        public string locale;
        public string utcOffset;
        public string last_modified_date;
        public bool is_app_installed;
        public Dictionary<string, string> custom_attributes;
    }
}

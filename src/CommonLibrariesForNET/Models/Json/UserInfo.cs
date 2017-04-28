using System.Collections.Generic;
using Newtonsoft.Json;

namespace Salesforce.Common.Models.Json
{
    public class UserInfo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id;

        [JsonProperty(PropertyName = "asserted_user")]
        public bool AssertedUser;

        [JsonProperty(PropertyName = "user_id")]
        public string UserId;

        [JsonProperty(PropertyName = "organization_id")]
        public string OrganizationId;

        [JsonProperty(PropertyName = "username")]
        public string Username;

        [JsonProperty(PropertyName = "nick_name")]
        public string NickName;

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName;

        [JsonProperty(PropertyName = "email")]
        public string Email;

        [JsonProperty(PropertyName = "email_verified")]
        public bool EmailVerified;

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName;

        [JsonProperty(PropertyName = "last_name")]
        public string LastName;

        [JsonProperty(PropertyName = "status")]
        public Dictionary<string, string> Status;

        [JsonProperty(PropertyName = "Photos")]
        public Dictionary<string, string> Photos;

        [JsonProperty(PropertyName = "AddressStreet")]
        public string AddressStreet;

        [JsonProperty(PropertyName = "AddressCity")]
        public string AddressCity;

        [JsonProperty(PropertyName = "AddressState")]
        public string AddressState;

        [JsonProperty(PropertyName = "AddressCountry")]
        public string AddressCountry;

        [JsonProperty(PropertyName = "addr_zip")]
        public string AddressZip;

        [JsonProperty(PropertyName = "mobile_phone")]
        public string MobilePhone;

        [JsonProperty(PropertyName = "mobile_phone_verified")]
        public bool MobilePhoneVerified;

        [JsonProperty(PropertyName = "urls")]
        public Dictionary<string, string> Urls;

        [JsonProperty(PropertyName = "active")]
        public bool Active;

        [JsonProperty(PropertyName = "user_type")]
        public string UserType;

        [JsonProperty(PropertyName = "language")]
        public string Language;

        [JsonProperty(PropertyName = "locale")]
        public string Locale;

        [JsonProperty(PropertyName = "utcOffset")]
        public string UtcOffset;

        [JsonProperty(PropertyName = "last_modified_date")]
        public string LastModifiedDate;

        [JsonProperty(PropertyName = "is_app_installed")]
        public bool IsAppInstalled;

        [JsonProperty(PropertyName = "custom_attributes")]
        public Dictionary<string, string> CustomAttributes;
    }
}

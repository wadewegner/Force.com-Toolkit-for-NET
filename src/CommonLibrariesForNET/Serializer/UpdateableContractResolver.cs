using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Salesforce.Common.Attributes;

namespace Salesforce.Common.Serializer
{
    public class UpdateableContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                                .Where(p => IsPropertyUpdateable(type, p))
                                .ToList();
        }

        private static bool IsPropertyUpdateable(Type type, JsonProperty property)
        {
            var isUpdateable = true;
            var propInfo = type.GetRuntimeProperty(property.PropertyName);

            if (propInfo != null)
            {
                var updateableAttr = propInfo.GetCustomAttribute(typeof(UpdateableAttribute), false);
                isUpdateable = updateableAttr == null || ((UpdateableAttribute)updateableAttr).Updateable;
            }

            return isUpdateable;
        }
    }
}
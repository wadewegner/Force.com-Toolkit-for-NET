using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Salesforce.Common.Attributes;

namespace Salesforce.Common.Serializer
{
    public class CreateableContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                                .Where(p => IsPropertyCreatable(type, p))
                                .ToList();
        }

        private static bool IsPropertyCreatable(Type type, JsonProperty property)
        {
            var isCreateable = true;
            var propInfo = type.GetRuntimeProperty(property.PropertyName);

            if (propInfo != null)
            {
                var createableAttr = propInfo.GetCustomAttribute(typeof(CreateableAttribute), false);
                isCreateable = createableAttr == null || ((CreateableAttribute)createableAttr).Createable;
            }

            return isCreateable;
        }
    }
}
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
            var properties = base.CreateProperties(type, memberSerialization);
            var updateableProperties = new List<JsonProperty>();

            foreach (var property in properties)
            {
                foreach (var o in type
                    .GetRuntimeProperty(property.PropertyName)
                    .GetCustomAttributes(typeof(UpdateableAttribute), false))
                {
                    var a = (UpdateableAttribute)o;
                    if (a.Updateable)
                    {
                        updateableProperties.Add(property);
                    }
                }
            }

            var additionalUpdateableProperties = properties
                .Where(p => !type.GetRuntimeProperty(p.PropertyName).GetCustomAttributes(typeof(UpdateableAttribute), false).Any())
                .ToList();

            updateableProperties.AddRange(additionalUpdateableProperties);

            return updateableProperties;
        }
    }
}
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
            var properties = base.CreateProperties(type, memberSerialization);
            var createableProperties = new List<JsonProperty>();

            foreach (var property in properties)
            {
                foreach (var o in type
                    .GetRuntimeProperty(property.PropertyName)
                    .GetCustomAttributes(typeof(CreateableAttribute), false))
                {
                    var a = (CreateableAttribute)o;
                    if (a.Createable)
                    {
                        createableProperties.Add(property);
                    }
                }
            }

            var additionalCreateableProperties = properties
                .Where(p => !type.GetRuntimeProperty(p.PropertyName).GetCustomAttributes(typeof(CreateableAttribute), false).Any())
                .ToList();

            createableProperties.AddRange(additionalCreateableProperties);

            return createableProperties;
        }
    }
}
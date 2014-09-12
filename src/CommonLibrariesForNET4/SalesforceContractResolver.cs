using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Salesforce.Common
{
    public class SalesforceContractResolver : DefaultContractResolver
    {
        public SalesforceContractResolver()//(List<string> propertiesToSerialize)
        {
            //this.mPropertiesToSerialize = propertiesToSerialize;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> objProperties = new List<JsonProperty>();
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            //objProperties = properties.Where(p => mPropertiesToSerialize.Contains(p.PropertyName)).ToList();

            //foreach (var jProperty in properties)
            //{
            //    if (!jProperty.PropertyType.IsClass)
            //        continue;

            //    var nestedObjProperties = base.CreateProperties(jProperty.PropertyType, memberSerialization);
            //    foreach (var nestedjProperty in nestedObjProperties)
            //    {
            //        if (objProperties.Any(nj => (nj.PropertyName == nestedjProperty.PropertyName)))
            //            continue;

            //        if (mPropertiesToSerialize.Contains(nestedjProperty.PropertyName))
            //        {
            //            objProperties.Add(nestedjProperty);
            //        }
            //    }
            //}
            return objProperties;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using Salesforce.Common.Attributes;

namespace Salesforce.Force
{
    public class ForceQueryBuilder
    {
        public static string DeriveQuery<T>(string objectName, string recordId)
        {
            if (objectName == null) throw new ArgumentNullException("objectName");
            if (recordId == null) throw new ArgumentNullException("recordId");

            var fieldNames = DeriveFieldsFromObject<T>();
            var fields = string.Join(", ", fieldNames);

            return string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
        }

        private static IEnumerable<string> DeriveFieldsFromObject<T>()
        {
            return DeriveFieldsFromObject(typeof(T), "");
        }

        private static IEnumerable<string> DeriveFieldsFromObject(Type objectType, string prefix)
        {
            foreach (var p in objectType.GetRuntimeProperties())
            {
                if (p.GetCustomAttribute<SubEntityAttribute>() == null)
                {
                    yield return prefix + p.Name;
                }
                else
                {
                    foreach (var s in DeriveFieldsFromObject(p.PropertyType, p.Name + "."))
                    {
                        yield return s;
                    }
                }
            }
        }
    }
}
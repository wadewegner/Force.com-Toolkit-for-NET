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
            var fieldNames = DeriveFieldsFromObject<T>();
            var fields = string.Join(", ", fieldNames);

            return string.Format("SELECT {0} FROM {1} WHERE Id = '{2}'", fields, objectName, recordId);
        }

        private static IEnumerable<string> DeriveFieldsFromObject<T>()
        {
            return DeriveFieldsFromObject(typeof(T));
        }

        private static IEnumerable<string> DeriveFieldsFromObject(Type propertyType, string prefix = "")
        {
            var allProperties = propertyType.GetRuntimeProperties();

            var fieldNames = new List<string>();
            foreach (var propertyInfo in allProperties)
            {
                if (propertyInfo.GetCustomAttribute<SubEntityAttribute>() != null)
                {
                    fieldNames.AddRange(DeriveFieldsFromObject(propertyInfo.PropertyType, propertyInfo.PropertyType.Name + "."));
                    continue;
                }
                fieldNames.Add(string.Format("{0}{1}", prefix, propertyInfo.Name));
            }

            return fieldNames;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Salesforce.Common.Serializer
{
    public static class CsvSerializer
    {
        public static string SerializeList<T>(List<T> objectList)
        {
            if (objectList == null || objectList.Count == 0) throw new ArgumentNullException("objectList");
            var type = objectList[0].GetType();
            var nameToCsvNameDictionary = GetCsvNameDictionaryForObjectProperties(type);
            var optIn = IsObjectTypeOptIn(type);

            var returnString = new StringBuilder();

            var workingDictionary = nameToCsvNameDictionary;
            if (optIn)
            {
                workingDictionary = nameToCsvNameDictionary.Where(entry => entry.Value != null)
                    .ToDictionary(entry => entry.Key, entry => entry.Value);
            }
            var keyOrderList = workingDictionary.Keys.ToList();

            for (var i = 0; i < keyOrderList.Count(); i++)
            {
                var key = keyOrderList[i];

                returnString.Append(workingDictionary[key] ?? key);
                if (i == keyOrderList.Count() - 1)
                {
                    returnString.Append("\n");
                }
                else
                {
                    returnString.Append(",");
                }
            }

            foreach (var obj in objectList)
            {
                for (var i = 0; i < keyOrderList.Count(); i++)
                {
                    var key = keyOrderList[i];
                    returnString.Append(GetPropertyValueByName(key, obj));
                    if (i == keyOrderList.Count() - 1)
                    {
                        returnString.Append("\n");
                    }
                    else
                    {
                        returnString.Append(",");
                    }
                }
            }

            return returnString.ToString();
        }

        [AttributeUsage(AttributeTargets.Property)]
        public sealed class CsvName : Attribute
        {
            internal string Name { get; private set; }
            public CsvName(string csvName)
            {
                Name = csvName;
            }
        }

        [AttributeUsage(AttributeTargets.Class)]
        public sealed class CsvOptIn : Attribute
        {
        }

        private static object GetPropertyValueByName(string name, object objectInstance)
        {
            var propertyInfo = objectInstance.GetType().GetRuntimeProperty(name);
            return propertyInfo.GetValue(objectInstance);
        }

        private static bool IsObjectTypeOptIn(Type t)
        {
            var typeInfo = t.GetTypeInfo();
            var optInAttributes = typeInfo.GetCustomAttributes<CsvOptIn>();
            return optInAttributes.Any();
        }

        private static Dictionary<string,string> GetCsvNameDictionaryForObjectProperties(Type t)
        {
            var properties = t.GetRuntimeProperties();
            return properties.Select(GetNameToCsvName).ToDictionary(nameAndCsv => nameAndCsv.Item1, nameAndCsv => nameAndCsv.Item2);
        }

        private static Tuple<string,string> GetNameToCsvName(PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            string csvName = null;

            var attributes = propertyInfo.GetCustomAttributes(typeof(CsvName), false).ToList();
            if (attributes.Count() == 1)
            {
                var csvAttribute = (CsvName)attributes[0];
                csvName = csvAttribute.Name;
            }

            return new Tuple<string,string>(name, csvName);
        }
    }
}

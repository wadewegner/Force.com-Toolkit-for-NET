using System;

namespace Salesforce.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SubEntityAttribute : Attribute
    {
    }
}
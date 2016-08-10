using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Salesforce.Common.Soql
{
	public class JsonPropertySelectListResolver : ISelectListResolver
	{
		private bool _ignorePropsWithoutAttribute;
		public JsonPropertySelectListResolver(bool ignorePropsWithoutAttribute)
		{
			_ignorePropsWithoutAttribute = ignorePropsWithoutAttribute;
		}

		public string GetFieldsList<T>()
		{
			var propInfo = typeof(T).GetRuntimeProperties();
			
			if (_ignorePropsWithoutAttribute)
				propInfo = propInfo.Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() != null);

			var fields = propInfo.Select(p => {
				var customAttribute = p.GetCustomAttribute<JsonPropertyAttribute>();
				return (customAttribute == null || customAttribute.PropertyName == null) ? p.Name : customAttribute.PropertyName;
			});

			return string.Join(", ", fields);
		}
	}
}

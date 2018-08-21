using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Salesforce.Common.Soql
{
	public class DataMemberSelectListResolver : ISelectListResolver
	{
		public string GetFieldsList<T>()
		{
			var fields = typeof(T).GetRuntimeProperties()
				.Where(p => {
					var customAttribute = p.GetCustomAttribute<IgnoreDataMemberAttribute>();
					return (customAttribute == null);
				})
				.Select(p => {
					var customAttribute = p.GetCustomAttribute<DataMemberAttribute>();
					return (customAttribute == null || customAttribute.Name == null) ? p.Name : customAttribute.Name;
				});

			return string.Join(", ", fields);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.Common.Soql
{
	public interface ISelectListResolver
	{
		string GetFieldsList<T>();
	}
}

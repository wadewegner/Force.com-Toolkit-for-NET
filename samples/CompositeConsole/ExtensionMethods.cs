using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositeConsole
{
    public static class ExtensionMethods
    {
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (int i = 0; i < (source.Count / size) + (source.Count % size > 0 ? 1 : 0); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}

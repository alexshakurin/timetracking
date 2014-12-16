using System.Collections.Generic;
using System.Linq;

namespace TimeTracking.Extensions
{
	public static class SetExtensions
	{
		public static bool In<T>(this T item, IEnumerable<T> collection)
		{
			var list = collection as IList<T>;
			if (list != null)
			{
				return list.Contains(item);
			}

			return collection.Any(c => c.Equals(item));
		}
	}
}
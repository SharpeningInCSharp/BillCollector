using System.Collections.Generic;
using System.Linq;

namespace DataBaseContext.OutputTools
{
	internal static class Helper
	{
		public static bool Contains(this List<GoodUseFrequence> goodUses, string name)
		{
			return goodUses.Count(x => x.Name == name) > 0;
		}

		public static GoodUseFrequence GetItem(this List<GoodUseFrequence> goodUses, string name)
		{
			return goodUses.First(x => x.Name == name);
		}

		public static T GetItem<T>(this IEnumerable<T> items, T currentItem)
		{
			foreach(var item in items)
			{
				if(item.Equals(currentItem))
				{
					return item;
				}
			}

			return default;
		}
	}
}

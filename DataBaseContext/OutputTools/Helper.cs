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
	}
}

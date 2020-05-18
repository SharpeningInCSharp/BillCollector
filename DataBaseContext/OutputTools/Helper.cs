using System.Collections.Generic;
using System.Linq;

namespace DataBaseContext.OutputTools
{
	internal static class Helper
	{
		public static bool Contains(this List<ExpenceLogManager.GoodUseFrequence> goodUses, string name)
		{
			return goodUses.Count(x => x.Name == name) > 0;
		}

		public static ExpenceLogManager.GoodUseFrequence GetItem(this List<ExpenceLogManager.GoodUseFrequence> goodUses, string name)
		{
			return goodUses.First(x => x.Name == name);
		}
	}
}

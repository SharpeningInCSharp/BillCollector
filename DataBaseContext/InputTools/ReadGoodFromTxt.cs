using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoodInfo;

namespace DataBaseContext.InputTools
{
	public static class ReadReceipFromTxt
	{
		public static IEnumerable<Good> Read(string path)
		{
			using var sr = new StreamReader(path);
			while(sr.EndOfStream is false)
			{
				var line = sr.ReadLine();
				Tuple<GoodType, string, decimal> tuple = TryParse(line);
				if (tuple is null)
					continue;

				yield return new Good(tuple.Item1, tuple.Item2, tuple.Item3);
			}
		}

		private static Tuple<GoodType, string, decimal> TryParse(string line)
		{
			var arr = line.Split();
			if (decimal.TryParse(arr.Last(), out var res) && Enum.TryParse(typeof(GoodType), arr.First(), out var eRes))
			{
				var t = arr[1..^1];
				//first item is GoodType, last one is price. Words in the middlle are name
				var name = t.Aggregate((x, y) => x += " " + y);
				return new Tuple<GoodType, string, decimal>((GoodType)eRes, name, res);
			}

			return null;
		}
	}
}

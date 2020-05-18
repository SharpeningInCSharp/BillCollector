using System;
using System.Collections.Generic;
using System.Text;

namespace GoodInfo
{
	public static class DataValidation
	{
		public static bool CheckPrice(string p)
		{
			if(decimal.TryParse(p, out var price))
			{
				return CheckPrice(price);
			}

			return false;
		}

		public static bool CheckPrice(decimal d)
		{
			if (d > 0 && d < (decimal)1e9)
				return true;

			return false;
		}

		public static bool CapitalName(string name)
		{
			if (name.Length == 0)
				return false;

			return name[0].Equals(char.ToUpper(name[0]));
		}

		public static bool CheckDiscount(decimal discount)
		{
			if (discount >= 0 && discount < 1)
				return true;

			return false;
		}
	}
}

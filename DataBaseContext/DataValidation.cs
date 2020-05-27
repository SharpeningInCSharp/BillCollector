using System;
using System.IO;

namespace DataBaseContext
{
	public static class DataValidation
	{
		public static int MaxPassLength = 18;
		public static int MaxLoginLength = 20;

		public static bool IsDateValid(DateTime date)
		{
			return date.Year > 2018 && date <= DateTime.Now;
		}

		public static bool BinaryFileExist(string path)
		{
			return File.Exists(path) && path.EndsWith(".dat");
		}

		public static bool IsPassValid(string pass)
		{
			return pass.Length <= MaxPassLength && pass.Length > 0;
		}

		public static bool IsLoginValid(string login)
		{
			return login.Length < MaxLoginLength && login.Length > 0;
		}
	}
}

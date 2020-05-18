using System;
using System.IO;

namespace DataBaseContext
{
	public static class DataValidation
	{
		public static bool IsDateValid(DateTime date)
		{
			return date.Year > 2018 && date <= DateTime.Now;
		}

		public static bool BinaryFileExist(string path)
		{
			return File.Exists(path) && path.EndsWith(".dat");
		}

	}
}

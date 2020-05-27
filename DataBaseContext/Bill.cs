using System;

namespace DataBaseContext
{
	public class Bill : Entity
	{
		public Bill(string path) : base(EntityType.Bill)
		{
			if (path is null)
				throw new ArgumentException($"{path} can't be null");

			Path = path;
		}

		public string Path { get; }
	}
}
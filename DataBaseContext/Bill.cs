using System;
using System.ComponentModel.DataAnnotations;

namespace DataBaseContext
{
	public class Bill : IEntity
	{
		internal Bill() : base(EntityType.Bill)
		{
			
		}

		public Bill(string path) : this()
		{
			if (path is null)
				throw new ArgumentException($"{path} can't be null");

			Path = path;
		}

		[Key]
		public int Id { get; set; }

		[Required]
		public string Path { get; }
	}
}
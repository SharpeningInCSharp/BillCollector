using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.Entities
{
	[Table("Bills")]
	internal class BillEntity
	{
		internal BillEntity()
		{ }

		[Key]
		public int Id { get; set; }

		[Required]
		public DateTime Created { get; set; }

		[Required]
		public string DataPath { get; set; }
	}
}

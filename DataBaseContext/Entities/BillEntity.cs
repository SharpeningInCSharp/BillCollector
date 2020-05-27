using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.Entities
{
	[Table("Receip")]
	internal class BillEntity
	{
		internal BillEntity()
		{ }

		[Key]
		public int Id { get; set; }

		[Required]
		public string DataPath { get; set; }
	}
}

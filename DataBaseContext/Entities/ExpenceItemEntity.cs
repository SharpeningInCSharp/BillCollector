using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.Entities
{
	[Table("ExpenceItem")]
	internal class ExpenceItemEntity
	{
		public ExpenceItemEntity()
		{

		}

		internal ExpenceItemEntity(GoodEntity good, int amount)
		{
			Good = good;
			Amount = amount;
		}

		[Key]
		public int Id { get; set; }

		public GoodEntity Good { get; set; }

		public int Amount { get; set; }
	}
}

using GoodInfo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.Entities
{
	[Table("Good")]
	internal class GoodEntity
	{
		public GoodEntity()
		{

		}

		internal GoodEntity(Good good)
		{
			Name = good.Name;
			Type = good.Type;
			Price = good.Price;
		}

		[Key]
		public int Id { get; set; }

		public string Name { get; set; }

		public GoodType Type { get; set; }

		public decimal Price { get; set; }
	}
}
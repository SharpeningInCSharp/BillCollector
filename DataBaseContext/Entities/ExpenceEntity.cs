using GoodInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.Entities
{
	[Table("Expence")]
	internal class ExpenceEntity
	{
		public ExpenceEntity()
		{	
		}

		[Key]
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]

		//public List<ExpenceItemEntity> Goods { get; set; }
		public Dictionary<Good, int > Goods { get; set; }

		public Guid? IdentityGuid { get; set; } = null;

		public BillEntity BillEntity { get; set; }
	}
}

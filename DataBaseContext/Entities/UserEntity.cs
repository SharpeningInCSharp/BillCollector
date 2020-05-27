using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseContext.Entities
{
	[Table("User")]
	internal class UserEntity
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Login { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public List<ExpenceEntity> Expences { get; set; }
	}
}

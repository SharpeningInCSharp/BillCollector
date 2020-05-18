using Microsoft.EntityFrameworkCore;

namespace DataBaseContext.Entities
{
	internal class DataBaseContext : DbContext
	{
		internal DbSet<BillEntity> Bills { get; set; }
		internal DbSet<ExpenceEntity> Expences { get; set; }

		internal DbSet<ExpenceItemEntity> ExpenceItemEntities { get; set; }

		internal DbSet<GoodEntity> GoodEntities { get; set; }

		public DataBaseContext()
		{
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BillCollector;Trusted_Connection=True;");
		}
	}
}

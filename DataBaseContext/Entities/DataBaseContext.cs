using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataBaseContext.Entities
{
	internal class BillsDataBaseContext : DbContext
	{
		internal DbSet<BillEntity> Bills { get; set; }

		internal DbSet<ExpenceEntity> Expences { get; set; }

		internal DbSet<ExpenceItemEntity> ExpenceItemEntities { get; set; }

		internal DbSet<GoodEntity> GoodEntities { get; set; }

		internal DbSet<UserEntity> Users { get; set; }

		public BillsDataBaseContext()
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ExpenceEntity>(b =>
			b.Property(e => e.Goods).HasConversion(
					s => JsonConvert.SerializeObject(s, Formatting.None),
					d => JsonConvert.DeserializeObject<List<ExpenceItemEntity>>(d)
					));

			modelBuilder.Entity<UserEntity>(u =>
			u.Property(p => p.Expences).HasConversion(
				s => JsonConvert.SerializeObject(s, Formatting.None),
				d => JsonConvert.DeserializeObject<List<ExpenceEntity>>(d)
				));

			base.OnModelCreating(modelBuilder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BillCollector-DB;Trusted_Connection=True;");
		}
	}
}

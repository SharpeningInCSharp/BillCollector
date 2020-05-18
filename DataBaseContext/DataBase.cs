using DataBaseContext.Entities;
using GoodInfo;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DataBaseContext
{
	public static class DataBase
	{
		public static async Task<bool> AddAsync(IEntity data)
		{
			return await Task.Run(() => Add(data));
		}

		public static List<Expence> Select(DateTime currentDate)
		{
			using var db = new Entities.DataBaseContext();
			return db.Expences.Where(x => x.Date == currentDate).Select(x => new Expence(x)).ToList();
		}

		public static IEnumerable<Expence> Select(DateTime initialDate, DateTime finalDate)
		{
			using var db = new Entities.DataBaseContext();
			return db.Expences.Where(x => x.Date >= initialDate && x.Date <= finalDate).Select(x => new Expence(x));
		}

		private static bool Add(IEntity data)
		{
			using var db = new Entities.DataBaseContext();
			if (CheckExictance(db, data))
			{
				var entity = ToEntityType(data);
				db.Add(entity);
				db.SaveChanges();
				return true;
			}

			return false;
		}

		private static object ToEntityType(IEntity data)
		{
			return data.EntityType switch
			{
				EntityType.Bill when data is Bill bill => new BillEntity
				{
					DataPath = bill.Path,
				},

				EntityType.Expence when data is Expence expence => new ExpenceEntity
				{
					Date = expence.Date,
					Goods = ToEntityGoodList(expence.Goods),
					IdentityGuid = expence.IdentityGuid,
					BillEntity = ToEntityType(expence.Bill) as BillEntity,
				},

				_ => new ArgumentException(),
			};
		}

		private static List<ExpenceItemEntity> ToEntityGoodList(Dictionary<Good, int> goods)
		{
			var result = new List<ExpenceItemEntity>();
			foreach (var item in goods)
			{
				result.Add(new ExpenceItemEntity(new GoodEntity(item.Key), item.Value));
			}

			return result;
		}

		private static bool CheckExictance(Entities.DataBaseContext db, IEntity entity)
		{
			switch (entity.EntityType)
			{
				case EntityType.Bill when entity is Bill bill:
					return db.Bills.ToList().Count(b => b.DataPath == bill.Path) == 0;

				case EntityType.Expence when entity is Expence expence:
					return db.Expences.ToList().Count(e => e.IdentityGuid == expence.IdentityGuid) == 0;

				default:
					throw new ArgumentException();
			}

			//return entity.EntityType switch
			//{
			//	EntityType.Bill when entity is Bill bill =>
			//					db.Bills.ToList().Count(b => b.DataPath == bill.Path) == 0,
			//	EntityType.Expence when entity is Expence expence =>
			//					db.Expences.ToList().Count(e => e.IdentityGuid == expence.IdentityGuid) == 0,
			//	_ => throw new ArgumentException(),
			//};
		}
	}
}

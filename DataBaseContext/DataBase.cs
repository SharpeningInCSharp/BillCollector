using DataBaseContext.Entities;
using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBaseContext.OutputTools;

namespace DataBaseContext
{
	public static class DataBase
	{
		public static async Task<bool> AddAsync(IEntity data)
		{
			return await Task.Run(() => Add(data));
		}

		public static IEnumerable<Expence> Select(DateTime currentDate)
		{
			using var db = new Entities.DataBaseContext();
			return db.Expences.Where(x => x.Date.Day == currentDate.Day).Select(x => new Expence(x)).ToList();
		}

		public static IEnumerable<Expence> Select(DateTime initialDate, DateTime finalDate)
		{
			//TODO: remake this request
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

		//TODO: add extra tables: ExpenceEntityItem...
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
			return entity.EntityType switch
			{
				EntityType.Bill when entity is Bill bill =>
								db.Bills.ToList().Count(b => b.DataPath == bill.Path) == 0,
				EntityType.Expence when entity is Expence expence =>
								db.Expences.ToList().Count(e => e.IdentityGuid == expence.IdentityGuid) == 0,
				_ => throw new ArgumentException(),
			};
		}

		public static IEnumerable<Expence.ExpenceSelection> SelectAndDistinct(GoodType goodType, DateTime initialDate, DateTime? finalDate)
		{
			//TODO: check selection requests
			IEnumerable<List<Expence.ExpenceSelection>> items;
			if (finalDate.HasValue)
			{
				items = Select(initialDate, finalDate.Value).Select(x => x.SelectAll());
			}
			else
			{
				items = Select(initialDate).Select(x => x.SelectAll());
			}

			Distinct(items, out List<Expence.ExpenceSelection> result);

			return result;
		}

		private static void Distinct(IEnumerable<List<Expence.ExpenceSelection>> items, out List<Expence.ExpenceSelection> result)
		{
			result = new List<Expence.ExpenceSelection>();

			foreach (var purchase in items)
			{
				foreach (var item in purchase)
				{
					if (result.Contains(item))
					{
						result.GetItem(item).IncreaseAmount(item.Amount);
					}
					else
					{
						result.Add(item);
					}
				}
			}
		}
	}
}


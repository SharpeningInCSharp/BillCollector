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
			var ans = db.Expences.Where(x => x.Date.Day == currentDate.Day).ToList();
			return ans.Select(x => new Expence(x));
		}

		public static IEnumerable<Expence> Select(DateTime initialDate, DateTime finalDate)
		{
			using var db = new Entities.DataBaseContext();
			return db.Expences.Where(x => x.Date >= initialDate && x.Date <= finalDate).Select(x => new Expence(x));
		}

		private static bool Add(IEntity data)
		{
			using var db = new Entities.DataBaseContext();
			if (CheckExistance(db, data))
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
					Goods = /*ToEntityGoodList(expence.Goods)*/expence.Goods,
					IdentityGuid = expence.IdentityGuid,
					BillEntity = ToEntityType(expence.Bill) as BillEntity,
				},

				_ => new ArgumentException(),
			};
		}

		private static List<ExpenceItemEntity> ToEntityGoodList(Dictionary<Good, int> goods)
		{
			var result = new List<ExpenceItemEntity>();
			//using var db = new Entities.DataBaseContext();
			foreach (var item in goods)
			{
				var good = new GoodEntity(item.Key);
				//if (CheckAdditionalItemsExistance(db, good) != -1)
				//{
				//	good = db.GoodEntities.ToList().Single(GoodExistenceCondition(good));
				//}
				//else
				//{
				//	db.GoodEntities.Add(good);
				//}

				var dicItem = new ExpenceItemEntity(good, item.Value);
				//if (CheckAdditionalItemsExistance(db, dicItem) != -1)
				//{
				//	dicItem = db.ExpenceItemEntities.ToList().Single(i => i.Amount == dicItem.Amount &&
				//															AreEqual(i.Good, dicItem.Good) == true);
				//}
				//else
				//{
				//	db.ExpenceItemEntities.Add(dicItem);
				//}

				result.Add(dicItem);
			}

			//db.SaveChanges();
			return result;
		}

		private static bool CheckExistance(Entities.DataBaseContext db, IEntity entity)
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

		private static int CheckAdditionalItemsExistance<T>(Entities.DataBaseContext db, T data)
		{
			if (data is GoodEntity goodEntity)
			{
				var item = db.GoodEntities.ToList().SingleOrDefault(GoodExistenceCondition(goodEntity));
				return item is null ? -1 : item.Id;
			}
			else if (data is ExpenceItemEntity expenceItemEntity)
			{
				var item = db.ExpenceItemEntities.ToList().SingleOrDefault(ExpenceItemCondition(expenceItemEntity));
				return item is null ? -1 : item.Id;
			}

			throw new ArgumentException($"Can't find Entity {data}");
		}

		private static Func<ExpenceItemEntity, bool> ExpenceItemCondition(ExpenceItemEntity expenceItemEntity) => i => i.Amount == expenceItemEntity.Amount &&
																													AreEqual(i.Good, expenceItemEntity.Good) == true;

		private static Func<GoodEntity, bool> GoodExistenceCondition(GoodEntity good2) => g => g.Name == good2.Name && g.Price == good2.Price && g.Type == good2.Type;

		private static bool AreEqual(GoodEntity good1, GoodEntity good2) => good1.Name == good2.Name && good1.Price == good2.Price && good1.Type == good2.Type;

		public static IEnumerable<Expence.ExpenceSelection> SelectAndDistinct(GoodType goodType, DateTime initialDate, DateTime? finalDate)
		{
			//SOLVE:goodType is unapplied
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


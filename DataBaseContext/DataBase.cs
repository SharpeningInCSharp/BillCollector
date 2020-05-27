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
		internal static void AddExpenceToAsync(string login, Expence expence)
		{
			if (LoginExist(login))
				throw new ArgumentException($"Can't find {login} user!");

			using var db = new Entities.BillsDataBaseContext();
			var user = db.Users.Single(x => x.Login == login);
			//SOLVE: how to add that shit
			//user.Expences.Add();
			db.SaveChanges();
		}

		//TODO: think about separated method to add User by login and pass

		internal static User UserExist(string login)
		{
			using var db = new Entities.BillsDataBaseContext();
			var item = db.Users.SingleOrDefault(x => x.Login == login);

			return item == null ? null : new User(item);
		}

		public static bool LoginExist(string login)
		{
			using var db = new Entities.BillsDataBaseContext();
			return db.Users.Count(x => x.Login == login) == 0;
		}

		public static async Task<bool> AddAsync(Entity data)
		{
			return await Task.Run(() => Add(data));
		}

		public static IEnumerable<Expence> Select(DateTime currentDate)
		{
			using var db = new Entities.BillsDataBaseContext();
			var ans = db.Expences.Where(x => x.Date.Day == currentDate.Day).ToList();
			return ans.Select(x => new Expence(x));
		}

		public static IEnumerable<Expence> Select(DateTime initialDate, DateTime finalDate)
		{
			DateToPropriateType(ref finalDate);
			using var db = new Entities.BillsDataBaseContext();
			var res = db.Expences.Where(x => x.Date >= initialDate && x.Date <= finalDate).ToList();
			return res.Select(x => new Expence(x));
		}

		/// <summary>
		/// Sets time to the end of day - 23:59:59
		/// </summary>
		/// <param name="date1">date to be edited</param>
		private static void DateToPropriateType(ref DateTime date1)
		{
			var dS = 59 - date1.Second;     //seconds delta to minute
			var dM = 59 - date1.Minute;     //minutes delta to midnight
			var dH = 23 - date1.Hour;       //hours delta to hour

			date1 = date1.AddHours(dH).AddMinutes(dM).AddSeconds(dS);
		}

		/// <summary>
		/// Returns expence dates
		/// </summary>
		/// <returns>IEnumerable of uniq dates</returns>
		public static IEnumerable<DateTime> GetAvailableExpenceDates()
		{
			using var db = new Entities.BillsDataBaseContext();
			var res = db.Expences.ToList();
			return res.Select(x => x.Date).Distinct();
		}

		private static bool Add(Entity data)
		{
			using var db = new Entities.BillsDataBaseContext();
			if (CheckExistance(db, data))
			{
				var entity = ToEntityType(data);
				db.Add(entity);
				db.SaveChanges();
				return true;
			}

			return false;
		}

		public static IEnumerable<Tuple<DateTime, string>> GetBills(DateTime current)
		{
			using var db = new Entities.BillsDataBaseContext();

			var res = from e in db.Expences.ToList()
					  where e.Date == current
					  join b in db.Bills.ToList() on e.BillEntityId equals b.Id
					  select new Tuple<DateTime, string>(e.Date, b.DataPath);

			return res;
		}

		public static IEnumerable<Tuple<DateTime, string>> GetBills(DateTime initial, DateTime final)
		{
			using var db = new Entities.BillsDataBaseContext();
			var items = db.Expences.ToList();

			DateToPropriateType(ref final);

			var res = from e in db.Expences.ToList()
					  where e.Date >= initial && e.Date <= final
					  join b in db.Bills.ToList() on e.BillEntityId equals b.Id
					  select new Tuple<DateTime, string>(e.Date, b.DataPath);

			return res;
		}

		public static DateTime GetLastExpenceDate()
		{
			using var db = new Entities.BillsDataBaseContext();
			var items = db.Expences.ToList();
			return items.Select(x => x.Date).OrderByDescending(x => x).First();
		}

		private static object ToEntityType(Entity data)
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
					Goods = ToEntityGoodList(expence.Goods),//expence.Goods,
					IdentityGuid = expence.IdentityGuid,
					BillEntity = ToEntityType(expence.Bill) as BillEntity,
				},

				EntityType.User when data is User user=> new UserEntity
				{
					Login = user.Login,
					Password = user.Pass,
					Expences = FromExpenceList(user.Expences),
				},

				_ => new ArgumentException(),
			};
		}

		private static List<ExpenceEntity> FromExpenceList(List<Expence> expences)
		{
			var res = new List<ExpenceEntity>(expences.Count);

			foreach(var item in expences)
			{
				res.Add(ToEntityType(item) as ExpenceEntity);
			}

			return res;
		}

		private static List<ExpenceItemEntity> ToEntityGoodList(Dictionary<Good, int> goods)
		{
			var result = new List<ExpenceItemEntity>();
			foreach (var item in goods)
			{
				var dicItem = new ExpenceItemEntity(new GoodEntity(item.Key), item.Value);

				result.Add(dicItem);
			}

			return result;
		}

		private static bool CheckExistance(Entities.BillsDataBaseContext db, Entity entity)
		{
			return entity.EntityType switch
			{
				EntityType.Bill when entity is Bill bill =>
								db.Bills.ToList().Count(b => b.DataPath == bill.Path) == 0,

				EntityType.Expence when entity is Expence expence =>
								db.Expences.ToList().Count(e => e.IdentityGuid == expence.IdentityGuid) == 0,

				EntityType.User when entity is User user =>
								db.Users.ToList().Count(u => u.Login == user.Login) == 0,

				_ => throw new ArgumentException(),
			};
		}

		public static IEnumerable<Expence.ExpenceSelection> SelectAndDistinct(GoodType goodType, DateTime initialDate, DateTime? finalDate)
		{
			IEnumerable<IEnumerable<Expence.ExpenceSelection>> items;
			if (finalDate.HasValue)
			{
				items = Select(initialDate, finalDate.Value).Select(x => x.SelectAll().Where(x => x.Type == goodType));
			}
			else
			{
				items = Select(initialDate).Select(x => x.SelectAll().Where(x => x.Type == goodType));
			}

			Distinct(items, out List<Expence.ExpenceSelection> result);

			return result;
		}

		private static void Distinct(IEnumerable<IEnumerable<Expence.ExpenceSelection>> items, out List<Expence.ExpenceSelection> result)
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

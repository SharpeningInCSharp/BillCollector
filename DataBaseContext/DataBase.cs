using DataBaseContext.Entities;
using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseContext
{
	public static class DataBase
	{
		public static User UserExist(string login)
		{
			using var db = new Entities.BillsDataBaseContext();
			var item = db.Users.FirstOrDefault(x => x.Login == login);

			return item == null ? null : new User(item);
		}

		public static async Task<bool> AddAsync(Entity data)
		{
			return await Task.Run(() => Add(data));
		}

		internal static ExpenceEntity GetExpence(Guid identity)
		{
			using var db = new BillsDataBaseContext();
			var item = (from e in db.Expences
						join b in db.Bills on e.BillEntityId equals b.Id
						select new ExpenceEntity
						{
							BillEntity = b,
							BillEntityId = e.BillEntityId,
							Date = e.Date,
							IdentityGuid = e.IdentityGuid,
							Goods = e.Goods,

						}).SingleOrDefault(x => x.IdentityGuid == identity);

			return item;
		}

		internal static void AddExpenceToUserAsync(string login, ExpenceEntity expenceEntity)
		{
			AddExpenceToUser(login, expenceEntity);
		}

		//НЕ ЛЕЗЬ, УБЬЕТ!
		private static void AddExpenceToUser(string login, ExpenceEntity expenceEntity)
		{
			using var db = new BillsDataBaseContext();
			var user = db.Users.Single(x => x.Login == login);

			var expE = new ExpenceEntity()
			{
				Date = expenceEntity.Date,
				Goods = expenceEntity.Goods,
				IdentityGuid = expenceEntity.IdentityGuid,
				BillEntity = expenceEntity.BillEntity,
			};

			var tempExpences = new List<ExpenceEntity>(user.Expences)
			{
				expE,
			};

			user.Expences = tempExpences;

			db.SaveChanges();
		}

		public static IEnumerable<Expense> Select(DateTime currentDate)
		{
			using var db = new Entities.BillsDataBaseContext();
			var ans = db.Expences.Where(x => x.Date.Day == currentDate.Day).ToList();
			return ans.Select(x => new Expense(x));
		}

		public static IEnumerable<Expense> Select(DateTime initialDate, DateTime finalDate)
		{
			DateToPropriateType(ref finalDate);
			using var db = new Entities.BillsDataBaseContext();
			var res = db.Expences.Where(x => x.Date >= initialDate && x.Date <= finalDate).ToList();
			return res.Select(x => new Expense(x));
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

			var res = (from e in db.Expences.ToList()
					   where e.Date.Year == current.Day && e.Date.Month == current.Month && e.Date.Day == current.Day
					   join b in db.Bills.ToList() on e.BillEntityId equals b.Id
					   select new Tuple<DateTime, string>(e.Date, b.DataPath)).ToList();

			return res;
		}

		public static IEnumerable<Tuple<DateTime, string>> GetBills(DateTime initial, DateTime final)
		{
			using var db = new Entities.BillsDataBaseContext();

			DateToPropriateType(ref final);

			var res = from e in db.Expences.ToList()
					  where e.Date >= initial && e.Date <= final
					  join b in db.Bills.ToList() on e.BillEntityId equals b.Id
					  select new Tuple<DateTime, string>(e.Date, b.DataPath);

			return res;
		}

		private static object ToEntityType(Entity data)
		{
			return data.EntityType switch
			{
				EntityType.Bill when data is Bill bill => new BillEntity
				{
					DataPath = bill.Path,
				},

				EntityType.Expense when data is Expense expence => new ExpenceEntity
				{
					Date = expence.Date,
					Goods = ToEntityGoodList(expence.Goods),//expence.Goods,
					IdentityGuid = expence.IdentityGuid,
					BillEntity = ToEntityType(expence.Bill) as BillEntity,
				},

				EntityType.User when data is User user => new UserEntity
				{
					Login = user.Login,
					Password = user.Pass,
					Expences = new List<ExpenceEntity>(),//FromExpenceList(user.Expences),
				},

				_ => new ArgumentException(),
			};
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

				EntityType.Expense when entity is Expense expence =>
								db.Expences.ToList().Count(e => e.IdentityGuid == expence.IdentityGuid) == 0,

				EntityType.User when entity is User user =>
								db.Users.ToList().Count(u => u.Login == user.Login) == 0,

				_ => throw new ArgumentException(),
			};
		}
	}
}

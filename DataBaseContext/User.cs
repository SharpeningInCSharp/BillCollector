using DataBaseContext.Entities;
using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseContext.OutputTools;

namespace DataBaseContext
{
	public partial class User : Entity
	{
		public delegate void UserAnouncerHandler(string message);

		private readonly UserAnouncerHandler AnouncerHandler;

		public string Login { get; }

		internal string Pass { get; private set; }

		public List<Expence> Expences { get; private set; }

		private User(string login, string pass) : base(EntityType.User)
		{
			Login = login;
			Pass = pass;
		}

		internal User(UserEntity userEntity) : this(userEntity.Login, userEntity.Password)
		{
			if (userEntity.Expences is null)
				Expences = new List<Expence>();
			else
				Expences = userEntity.Expences.Select(x => new Expence(x)).ToList();
		}

		public User(string login, string pass, UserAnouncerHandler anouncerHandler) : this(login, pass)
		{
			AnouncerHandler = anouncerHandler;

			OutMessage($"You're welcome, {Login}!");
		}

		public void AddExpence(Guid identity)
		{
			var expenceEntity = DataBase.GetExpence(identity);

			if (expenceEntity != null)
			{
				Expences.Add(new Expence(expenceEntity));
				DataBase.AddExpenceToUserAsync(Login, expenceEntity);
			}
		}

		public bool IsPassValid(string pass)
		{
			return Pass.Equals(pass);
		}

		private void OutMessage(string message)
		{
			AnouncerHandler?.Invoke(message);
		}

		public void ChangePass(string newPass)
		{
			Pass = newPass;
			OutMessage("Your password's been changed!");
		}
	}

	/// <summary>
	/// DataBase interraction logic
	/// </summary>
	public partial class User
	{
		public IEnumerable<ExpenceSelection> SelectAndDistinct(GoodType goodType, DateTime initialDate, DateTime? finalDate)
		{
			IEnumerable<IEnumerable<ExpenceSelection>> items;
			if (finalDate.HasValue)
			{
				items = Select(initialDate, finalDate.Value).Select(x => x.SelectAll().Where(x => x.Type == goodType));
			}
			else
			{
				items = Select(initialDate).Select(x => x.SelectAll().Where(x => x.Type == goodType));
			}

			Distinct(items, out List<ExpenceSelection> result);

			return result;
		}

		private static void Distinct(IEnumerable<IEnumerable<ExpenceSelection>> items, out List<ExpenceSelection> result)
		{
			result = new List<ExpenceSelection>();

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

		public IEnumerable<Expence> Select(DateTime currentDate)
		{
			return Expences.Where(x => x.Date.Day == currentDate.Day);
		}

		public IEnumerable<Expence> Select(DateTime initialDate, DateTime finalDate)
		{
			DateToPropriateType(ref finalDate);
			return Expences.Where(x => x.Date >= initialDate && x.Date <= finalDate);
		}

		private static void DateToPropriateType(ref DateTime date1)
		{
			var dS = 59 - date1.Second;     //seconds delta to minute
			var dM = 59 - date1.Minute;     //minutes delta to midnight
			var dH = 23 - date1.Hour;       //hours delta to hour

			date1 = date1.AddHours(dH).AddMinutes(dM).AddSeconds(dS);
		}

		public IEnumerable<DateTime> GetAvailableExpenceDates()
		{
			return Expences.Select(x => x.Date).Distinct();
		}

		public DateTime? GetLastExpenceDate()
		{
			var dates = Expences.Select(x => x.Date).OrderByDescending(x => x);

			if (dates.Count() == 0)
				return null;
			else
				return dates.First();
		}
	}
}

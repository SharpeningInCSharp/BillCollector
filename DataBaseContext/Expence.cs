using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseContext.OutputTools;
using DataBaseContext.Entities;

namespace DataBaseContext
{
	public class Expense : Entity
	{
		public decimal Sum => Goods.Sum(x => x.Key.Price * x.Value);

		public Expense() : base(EntityType.Expense)
		{
			Goods = new Dictionary<Good, int>();
			Date = DateTime.Now;
		}

		internal Expense(ExpenceEntity expenceEntity) : base(EntityType.Expense)
		{
			Goods = new Dictionary<Good, int>();

			foreach (var item in expenceEntity.Goods)
			{
				var g = new Good(item.Good.Type, item.Good.Name, item.Good.Price);
				Goods.Add(g, item.Amount);
			}

			Date = expenceEntity.Date;
			IdentityGuid = expenceEntity.IdentityGuid;
		}

		public DateTime Date { get; }

		internal Dictionary<Good, int> Goods { get; }

		public Guid? IdentityGuid { get; private set; } = null;

		public int ItemsCount => Goods.Count;

		public Bill Bill { get; private set; }

		public void CreateBill(string path)
		{
			Bill = new Bill(path);
		}

		public void Add(Good good)
		{
			if (Goods.Keys.Contains(good))
			{
				Goods[good]++;
			}
			else
			{
				Goods.Add(good, 1);
			}

			ExpenceLogManager.SaveDataAsync(good);
		}

		public IEnumerable<ExpenseSelection> SelectAll()
		{
			int temp = 0;
			return Goods.Select(x => new ExpenseSelection(++temp, x.Value, x.Key));
		}

		public void Remove(ExpenseSelection expence)
		{
			var item = Goods.FirstOrDefault(x => x.Key.Name == expence.Item && x.Key.Price == expence.Price).Key;
			if (item != null)
				Goods.Remove(item);
		}

		public void Clear()
		{
			Goods.Clear();
		}

		public IEnumerable<ExpenseSelection> GetGoodsByGoodType(GoodType goodType)
		{
			return Goods.Where(x => x.Key.Type == goodType).Select(x => new ExpenseSelection(0, x.Value, x.Key));
		}

		public void CreateGuid()
		{
			if (IdentityGuid.HasValue == false)
			{
				IdentityGuid = Guid.NewGuid();
			}
		}
	}
}
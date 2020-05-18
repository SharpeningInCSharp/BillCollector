using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseContext.OutputTools;
using DataBaseContext.Entities;

namespace DataBaseContext
{
	public class Expence : IEntity
	{
		public decimal Sum => Goods.Sum(x => x.Key.Price * x.Value);

		public Expence() : base(EntityType.Expence)
		{
			Date = DateTime.Now;
		}

		internal Expence(ExpenceEntity expenceEntity) : base(EntityType.Expence)
		{
			foreach (var item in expenceEntity.Goods)
			{
				var g = new Good(item.Good.Type, item.Good.Name, item.Good.Price);
				Goods.Add(g, item.Amount);
			}

			Date = expenceEntity.Date;
			IdentityGuid = expenceEntity.IdentityGuid;
		}

		public DateTime Date { get; }

		internal Dictionary<Good, int> Goods => new Dictionary<Good, int>();

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

		public List<ExpenceSelection> SelectAll()
		{
			int temp = 0;
			var itemsList = Goods.Select(x => new ExpenceSelection(++temp, x.Value, x.Key)).ToList();
			return itemsList;
		}

		public void Remove(ExpenceSelection expence)
		{
			var item = Goods.FirstOrDefault(x => x.Key.Name == expence.Item && x.Key.Price == expence.Price).Key;
			if (item != null)
				Goods.Remove(item);
		}

		public void Clear()
		{
			Goods.Clear();
		}

		public IEnumerable<GoodUseFrequence> GetGoodsByGoodType(GoodType goodType)
		{
			return Goods.Where(x => x.Key.Type == goodType).Select(x => new GoodUseFrequence(x.Key));
		}

		public void CreateGuid()
		{
			if (IdentityGuid.HasValue == false)
			{
				IdentityGuid = Guid.NewGuid();
			}
		}

		public class ExpenceSelection
		{
			internal ExpenceSelection(int num, int amount, Good good)
			{
				Num = num;
				this.item = good.Name;
				Price = good.Price;
				Amount = amount;
				this.good = good;
				TotalPrice = Price * amount;
			}

			public int Num { get; }

			private readonly Good good;
			private string item;

			public string Item
			{
				get => item;
				set
				{
					item = value;
					good?.Rename(item);
				}
			}

			public decimal Price { get; }

			public int Amount { get; private set; }

			public decimal TotalPrice { get; }
		}
	}
}
using GoodInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataBaseContext.OutputTools;
using DataBaseContext.Entities;

namespace DataBaseContext
{
	public class Expence : IEntity
	{
		//TODO: make extra internal entities Expence and Bill to work with but on the minimalAbstraction with opened get; set;
		public Expence() : base(EntityType.Expence)
		{
			Goods = new Dictionary<Good, int>();
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

		[Key]
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; }

		[Required]
		internal Dictionary<Good, int> Goods { get; private set; }

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
			var itemsList = Goods.Select(x => new ExpenceSelection(++temp, x.Key.Name, x.Key.Price, x.Value, x.Key)).ToList();
			return itemsList;
		}

		public Good First(string goodName)
		{
			return Goods.FirstOrDefault(x => x.Key.Name == goodName).Key;
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

		public void CreateGuid()
		{
			if (IdentityGuid.HasValue == false)
			{
				IdentityGuid = Guid.NewGuid();
			}
		}

		public class ExpenceSelection
		{
			internal ExpenceSelection(int num, string item, decimal price, int amount, Good good)
			{
				Num = num;
				this.item = item;
				Price = price;
				Amount = amount;
				this.good = good;
				TotalPrice = price * amount;
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

			public int Amount { get; }

			public decimal TotalPrice { get; }
		}
	}
}
using DataBaseContext.Diagrams;
using GoodInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataBaseContext
{

	public partial class ExpenceSelection
	{
		internal ExpenceSelection(int num, int amount, Good good)
		{
			Num = num;
			item = good.Name;
			Price = good.Price;
			Amount = amount;
			Type = good.Type;
			this.good = good;
		}

		public GoodType Type { get; }

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

		public decimal TotalPrice => Price * Amount;

		public void IncreaseAmount(int amount)
		{
			if (amount > 0)
				Amount += amount;
		}
	}

	public partial class ExpenceSelection : IEquatable<ExpenceSelection>, IScopeSelectionItem
	{
		public decimal GetTotal => TotalPrice;

		public override bool Equals(object obj)
		{
			if (obj is ExpenceSelection expence)
				return Equals(expence);

			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Item, Price, good.Type);
		}

		public override string ToString()
		{
			return $"{Amount} x {Item} - {Price}";
		}

		public bool Equals(ExpenceSelection other)
		{
			if (other is null)
				return false;

			return good.Equals(other.good);
		}
	}
}

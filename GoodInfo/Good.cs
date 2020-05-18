using System;
using System.Diagnostics.CodeAnalysis;

namespace GoodInfo
{
	public class Good : IEquatable<Good>, IComparable<Good>, IComparable
	{
		public string Name { get; private set; }

		public GoodType Type { get; private set; }

		public decimal Price { get; private set; }

		public Good(GoodType type, string name, decimal price)
		{
			if (DataValidation.CheckPrice(price) is false)
			{
				throw new ArgumentException($"Invalid {price}");
			}

			if (DataValidation.CapitalName(name) is false)
			{
				NameToCapital(ref name);
			}

			Type = type;
			Name = name;
			Price = price;
		}

		private void NameToCapital(ref string name)
		{
			name = char.ToUpper(name[0]) + name.Substring(1);
		}

		public void ChangeTypeTo(GoodType newType)
		{
			Type = newType;
		}

		public void Rename(string newName)
		{
			if (DataValidation.CapitalName(newName))
			{
				Name = newName;
			}
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Good good)
				return Equals(good);
			return false;
		}

		public bool Equals([AllowNull] Good other)
		{
			if (other is null)
				return false;

			return other.Type == Type && other.Name == Name && other.Price == Price;
		}

		public int CompareTo(Good other)
		{
			if (other is null)
				throw new ArgumentNullException($"Compared object {other} was null");

			if (Name.Equals(other.Name) && Type == other.Type && Price == other.Price)
			{
				return 0;
			}
			else
			{
				return Name.CompareTo(other.Name);
			}
		}

		public int CompareTo(object obj)
		{
			if (obj is Good good)
				return CompareTo(good);

			throw new ArgumentException($"Obj is not {typeof(Good)}");
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Type, Price);
		}
	}
}

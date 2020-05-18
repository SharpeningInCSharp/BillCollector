using System;
using System.Diagnostics.CodeAnalysis;

namespace GoodInfo
{
	/// <summary>
	/// Good abstraction
	/// </summary>
	public partial class Good
	{
		/// <summary>
		/// Good name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Good type
		/// </summary>
		public GoodType Type { get; private set; }

		/// <summary>
		/// Good price
		/// </summary>
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

		/// <summary>
		/// Name auto correction
		/// </summary>
		/// <param name="name">ref to input good name to be edited</param>
		private void NameToCapital(ref string name)
		{
			name = char.ToUpper(name[0]) + name.Substring(1);
		}

		/// <summary>
		/// Change type name
		/// </summary>
		/// <param name="newType">new goodType</param>
		public void ChangeTypeTo(GoodType newType)
		{
			Type = newType;
		}

		/// <summary>
		/// Change good name
		/// </summary>
		/// <param name="newName">new name</param>
		public void Rename(string newName)
		{
			if (DataValidation.CapitalName(newName)==false)
			{
				NameToCapital(ref newName);
			}

			Name = newName;
		}

	}

	/// <summary>
	/// Good interfaces part
	/// </summary>
	public partial class Good : IEquatable<Good>, IComparable<Good>, IComparable
	{
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

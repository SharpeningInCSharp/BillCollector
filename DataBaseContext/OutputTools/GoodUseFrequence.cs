using GoodInfo;
using System;

namespace DataBaseContext.OutputTools
{
	[Serializable]
	public sealed partial class GoodUseFrequence
	{
		public string Name { get; private set; }

		public int Times { get; private set; }

		public GoodType Type { get; private set; }

		internal GoodUseFrequence(GoodType goodType, string name, int times)
		{
			Name = name;
			Times = times;
			Type = goodType;
		}

		internal GoodUseFrequence(Good good)
		{
			Name = good.Name;
			Times = 1;
			Type = good.Type;
		}

		internal void IncreaseTime()
		{
			Times++;
		}

		public void ChangeName(string name)
		{
			if (GoodInfo.DataValidation.CapitalName(name))
				Name = name;
		}

		public void ChangeType(string type)
		{
			if (Enum.TryParse(typeof(GoodType), type, out var result))
			{
				Type = (GoodType)result;
			}
		}
	}

	public sealed partial class GoodUseFrequence : IEquatable<string>, IEquatable<GoodUseFrequence>
	{
		public override bool Equals(object obj)
		{
			if (obj is null)
				return false;

			return Equals(obj as GoodUseFrequence);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Type);
		}

		public bool Equals(string other)
		{
			return Name.Equals(other);
		}

		public bool Equals(GoodUseFrequence other)
		{
			return Equals(other.Name);
		}
	}
}

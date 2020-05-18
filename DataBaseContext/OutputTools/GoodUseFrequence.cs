using GoodInfo;
using System;

namespace DataBaseContext.OutputTools
{
	[Serializable]
	public class GoodUseFrequence : IEquatable<string>, IEquatable<GoodUseFrequence>
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

		public void IncreaseTimeFor(int times)
		{
			if (times > 0)
				Times += times;
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

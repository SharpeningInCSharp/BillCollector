using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseContext.OutputTools;

namespace DataBaseContext.Diagrams
{
	public class Range
	{
		public decimal Sum { get; private set; }
		public List<string> ItemsLog { get; private set; } = new List<string>();
		public DateTime InitialDate { get; }
		public DateTime? FinalDate { get; }

		//TODO: develop interface for working  with Diagram, using that class
		public Range(DateTime dateTime, GoodType goodType)
		{
			InitialDate = dateTime;
			Initialize(goodType);
		}

		public Range(DateTime initialDate, DateTime finalDate, GoodType goodType)
		{
			InitialDate = initialDate;
			FinalDate = finalDate;
			Initialize(goodType);
		}

		private void Initialize(GoodType goodType)
		{
			GetCollection(out IEnumerable<Expence> data);

			var totalPrice = data.Sum(x => x.Sum);

			var items = data.Select(x => x.GetGoodsByGoodType(goodType));

			Distinct(items);
		}

		private void Distinct(IEnumerable<IEnumerable<Expence.ExpenceSelection>> items)
		{
			var result = new List<Expence.ExpenceSelection>();
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

			ItemsLog = result.Select(x => x.ToString()).ToList();
		}

		private IEnumerable<Expence> GetCollection(out IEnumerable<Expence> data)
		{
			if (FinalDate.HasValue)
			{
				data = DataBase.Select(InitialDate, FinalDate.Value);
			}
			else
			{
				data = DataBase.Select(InitialDate);
			}

			return data;
		}
	}
}

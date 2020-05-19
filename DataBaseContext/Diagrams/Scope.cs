using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseContext.OutputTools;

namespace DataBaseContext.Diagrams
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DType">Data type</typeparam>
	public partial class Scope<DType>
				where DType : IScopeSelectionItem
	{
		public decimal Sum { get; private set; }

		private IEnumerable<DType> Items { get; }

		internal List<string> ItemsLog => Items.Select(x => x.ToString()).ToList();

		public DateTime InitialDate { get; }

		public DateTime? FinalDate { get; }

		internal Scope(IEnumerable<DType> items, DateTime dateTime)
		{
			Items = items;
			InitialDate = dateTime;
			Sum = Items.Sum(x => x.GetTotal);
		}

		internal Scope(IEnumerable<DType> items, DateTime initialDate, DateTime finalDate) : this(items, initialDate)
		{
			FinalDate = finalDate;
		}

		internal IEnumerable<DType> GetTopExpensive()
		{
			return Items.OrderByDescending(x => x.GetTotal).Take(3);
		}
	}

	public partial class Scope<DType> : IStringOutputData
	{
		/// <summary>
		/// Using Handler output line by line items
		/// </summary>
		/// <param name="OutputHandler"></param>
		public void OutputData(Action<string> OutputHandler)
		{
			foreach (var item in ItemsLog)
			{
				OutputHandler?.Invoke(item + "\n");
			}
		}
	}
}

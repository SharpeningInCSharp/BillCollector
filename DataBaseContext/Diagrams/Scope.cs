using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataBaseContext.Diagrams
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="EType">Enum class</typeparam>
	/// <typeparam name="DType">Data type</typeparam>
	public partial class Scope<EType, DType>
				where EType : Enum
				where DType : IScopeSelectionItem
	{
		public decimal Sum { get; private set; } = 0;

		public decimal PerCent { get; private set; } = 0;

		public EType EnumMember { get; private set; }

		private IEnumerable<DType> Items { get; }

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

		internal void SetPerCent(decimal perCent)
		{
			PerCent = Math.Round(perCent, 3);
		}

		internal void SetEnumMem(EType eType)
		{
			EnumMember = eType;
		}
	}

	public partial class Scope<EType, DType> : IStringOutputData
	{
		/// <summary>
		/// Using Handler output line by line items
		/// </summary>
		/// <param name="OutputHandler"></param>
		public void OutputData(Action<string, string> OutputHandler)
		{
			foreach (var item in Items)
			{
				OutputHandler?.Invoke(item.ToString(), item.GetTotal.ToString("C2"/*,CultureInfo.CreateSpecificCulture()*/));
			}
		}
	}
}

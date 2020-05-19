using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataBaseContext.Diagrams
{
	/// <summary>
	/// Provides interface for Scopes with simular date/range, enum 
	/// Contais general info about scopes and about each of them
	/// Capable to output general info about scopes and about each of them
	/// </summary>
	/// <typeparam name="EType">Enum type</typeparam>
	public partial class Scopes<EType, DType>
						where EType : Enum
						where DType : IScopeSelectionItem
	{
		public decimal TotalSum => scopes.Sum(x => x.Sum);
		public List<string> TotalInfo { get; private set; }
		public DateTime InitialDate { get; }
		public DateTime? FinalDate { get; }
		private readonly List<Scope<DType>> scopes = new List<Scope<DType>>();

		/// <summary>
		/// Scope for current date
		/// </summary>
		/// <param name="currentlDate"></param>
		/// <param name="enumeration">Enum of values</param>
		public Scopes(Func<EType, List<DType>> dataProvider, EType enumeration, DateTime currentlDate)
		{
			InitialDate = currentlDate;
			Initialize(enumeration, dataProvider);
		}

		/// <summary>
		/// Scope for range of dates
		/// </summary>
		/// <param name="initialDate">Start of range</param>
		/// <param name="finalDate">End of range</param>
		/// <param name="enumeration">Enum of values</param>
		public Scopes(Func<EType, List<DType>> dataProvider, EType enumeration, DateTime initialDate, DateTime finalDate)
		{
			InitialDate = initialDate;
			FinalDate = finalDate;
			Initialize(enumeration, dataProvider);
		}

		private void Initialize(EType enumeration, Func<EType, List<DType>> dataProvider)
		{
			var values = Enum.GetValues(enumeration.GetType());
			foreach (var value in values)
			{
				var result = dataProvider.Invoke((EType)value);
				Scope<DType> scope;
				if (FinalDate.HasValue)
				{
					scope = new Scope<DType>(result, InitialDate, FinalDate.Value);

				}
				else
				{
					scope = new Scope<DType>(result, InitialDate);
				}

				scopes.Add(scope);
			}
		}

	}

	public partial class Scopes<EType, DType> : IEnumerable<Scope<DType>>, IEnumerable, IStringOutputData
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return scopes.GetEnumerator();
		}

		public Scope<DType> this[int ind]
		{
			get
			{
				if (ind < 0 || ind >= scopes.Count)
					throw new ArgumentOutOfRangeException("Index was out of range");

				return scopes[ind];
			}
		}

		public IEnumerator<Scope<DType>> GetEnumerator()
		{
			return scopes.GetEnumerator();
		}

		/// <summary>
		/// Returns top-3 the most expensive items in each category
		/// </summary>
		/// <param name="OutputHandler">Handler for output</param>
		public void OutputData(Action<string> OutputHandler)
		{
			var categories = scopes.Select(x => x.GetTopExpensive().Select(x => x.ToString()));
			foreach (var category in categories)
			{
				foreach (var item in category)
				{
					OutputHandler?.Invoke(item);
				}
			}
		}

		//SOLVE: WARNING! Is total price updating with amount increasing?
	}
}

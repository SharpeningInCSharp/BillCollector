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
	/// <typeparam name="EType">Enum class</typeparam>
	/// <typeparam name="DType">Data type</typeparam>
	public partial class Scopes<EType, DType>
						where EType : Enum
						where DType : IScopeSelectionItem
	{
		public bool IsEmpty => TotalSum == 0;
		public DateTime InitialDate { get; }
		public DateTime? FinalDate { get; }
		public List<string> EnumStringValues { get; } = new List<string>();
		public Type EnumType { get; }
		public decimal TotalSum => scopes.Sum(x => x.Sum);

		public List<string> TotalInfo { get; private set; }

		private readonly List<Scope<EType, DType>> scopes = new List<Scope<EType, DType>>();

		/// <summary>
		/// Scope for range of dates
		/// </summary>
		/// <param name="initialDate">Start of range</param>
		/// <param name="finalDate">End of range</param>
		/// <param name="enumeration">Enum of values</param>
		public Scopes(Func<EType, DateTime, DateTime?, IEnumerable<DType>> dataProvider, Type enumType, DateTime initialDate, DateTime? finalDate)
		{
			if (enumType.IsEnum == false)
				throw new ArgumentException($"Type {enumType} must be Enum!");

			if (enumType != typeof(EType))
				throw new ArgumentException($"Enum type must be {typeof(EType)} not {enumType}");

			InitialDate = initialDate;
			FinalDate = finalDate;
			Initialize(enumType, dataProvider);
		}

		private void Initialize(Type enumType, Func<EType, DateTime, DateTime?, IEnumerable<DType>> dataProvider)
		{
			var values = Enum.GetValues(enumType);
			foreach (var value in values)
			{
				EnumStringValues.Add(value.ToString());
				var eType = (EType)value;
				var result = dataProvider.Invoke(eType, InitialDate, FinalDate);
				Scope<EType, DType> scope;
				if (FinalDate.HasValue)
				{
					scope = new Scope<EType, DType>(result, InitialDate, FinalDate.Value);

				}
				else
				{
					scope = new Scope<EType, DType>(result, InitialDate);
				}

				scope.SetEnumMem(eType);
				scopes.Add(scope);
			}

			if (IsEmpty == false)
				SetPerCents();
		}

		private void SetPerCents()
		{
			foreach (var item in scopes)
			{
				item.SetPerCent(item.Sum / TotalSum);
			}
		}
	}

	public partial class Scopes<EType, DType> : IEnumerable<Scope<EType, DType>>, IEnumerable, IStringOutputData
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return scopes.GetEnumerator();
		}

		public Scope<EType, DType> this[int ind]
		{
			get
			{
				if (ind < 0 || ind >= scopes.Count)
					throw new ArgumentOutOfRangeException("Index was out of range");

				return scopes[ind];
			}
		}

		public IEnumerator<Scope<EType, DType>> GetEnumerator()
		{
			return scopes.GetEnumerator();
		}

		/// <summary>
		/// Returns top-3 the most expensive items in each category
		/// </summary>
		/// <param name="OutputHandler">Handler for output</param>
		public void OutputData(Action<string, string> OutputHandler)
		{
			var categories = scopes.Select(x => x.GetTopExpensive());
			foreach (var category in categories)
			{
				foreach (var item in category)
				{
					OutputHandler?.Invoke(item.ToString(), item.GetTotal.ToString("C2"));
				}
			}
		}
	}
}

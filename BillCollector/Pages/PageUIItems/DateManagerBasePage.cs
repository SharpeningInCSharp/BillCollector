using DataBaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BillCollector.Pages.PageUIItems
{
	public abstract class DateManagerBasePage : Page
	{
		public Calendar Calendar { get; private set; }

		public static readonly DateTime InitialDate = new DateTime(2019, 1, 1);

		protected DateManagerBasePage()
		{ }

		protected void SetCaneldar(Calendar calendar)
		{
			Calendar = calendar;
		}

		private void CrossOutEmptyDatesAsync()
		{
			var avaialbleDates = DataBase.GetAvailableDates();
			var currentDate = DateTime.Today;
			while (currentDate != InitialDate)
			{
				if (avaialbleDates.Count(x => x.Year == currentDate.Year &&
										x.Month == currentDate.Month &&
										x.Day == currentDate.Day) == 0)     // if there's no such date
				{

					Dispatcher.Invoke(() => Calendar.BlackoutDates.Add(new CalendarDateRange(currentDate)));
				}

				currentDate = currentDate.AddDays(-1);
			}
		}

		protected async void Display()
		{
			Calendar.DisplayDateStart = new DateTime(InitialDate.Year, InitialDate.Month, InitialDate.Day);

			Initialize();

			await Task.Run(() => CrossOutEmptyDatesAsync());
		}

		protected abstract void Initialize();
	}
}

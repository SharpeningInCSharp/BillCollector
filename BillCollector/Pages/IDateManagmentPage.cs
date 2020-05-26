using DataBaseContext;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BillCollector.Pages
{
	internal interface IDateManagmentPage
	{
		public static readonly DateTime InitialDate = new DateTime(2019, 1, 1);

		internal void CrossOutEmptyDatesAsync(Dispatcher dispatcher, Calendar calendar)
		{
			var avaialbleDates = DataBase.GetAvailableDates();
			var currentDate = DateTime.Today;
			while (currentDate != InitialDate)
			{
				if (avaialbleDates.Count(x => x.Year == currentDate.Year &&
										x.Month == currentDate.Month &&
										x.Day == currentDate.Day) == 0)     // if there's no such date
				{
					dispatcher.Invoke(() => calendar.BlackoutDates.Add(new CalendarDateRange(currentDate)));
				}

				currentDate = currentDate.AddDays(-1);
			}
		}
	}
}

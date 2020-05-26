using BillCollector.Pages.PageUIItems;
using DataBaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BillCollector.Pages
{
	/// <summary>
	/// Логика взаимодействия для BillsPage.xaml
	/// </summary>
	public partial class BillsPage : Page
	{
		private readonly DateTime InitialDate = new DateTime(2019, 1, 1);

		public BillsPage()
		{
			InitializeComponent();

			Display();
		}

		private async void Display()
		{
			Calendar.DisplayDateStart = new DateTime(InitialDate.Year, InitialDate.Month, InitialDate.Day);
			Calendar.DisplayDateEnd = DateTime.Today;

			DateTime lastExpenceD;
			Calendar.SelectedDate = lastExpenceD = DataBase.GetLastExpenceDate();
			Load(lastExpenceD, null);

			//'cause DateRange can be really big
			await Task.Run(() => CrossOutEmptyDatesAsync());
		}

		private void CrossOutEmptyDatesAsync()
		{
			var avaialbleDates = DataBase.GetAvailableExpenceDates();
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

		private void Load(DateTime initialDate, DateTime? finalDate)
		{
			Init(initialDate, finalDate, out IEnumerable<Tuple<DateTime, string>> items);

			BillsListView.Children.Clear();
			foreach (var item in items)
			{
				var uiItem = new BillsListViewItem(item.Item1, item.Item2);
				BillsListView.Children.Add(uiItem);
			}
		}

		private static void Init(DateTime initialDate, DateTime? finalDate, out IEnumerable<Tuple<DateTime, string>> items)
		{
			if (finalDate.HasValue)
			{
				items = DataBase.GetBills(initialDate, finalDate.Value);
			}
			else
			{
				items = DataBase.GetBills(initialDate);
			}

		}

		private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			Mouse.Capture(null);

			var dates = Calendar.SelectedDates.OrderBy(x => x.Date);
			DateTime initial = dates.First();
			DateTime? final = dates.Last();

			Load(initial, final);
		}

		private void LoadFreshBill_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}

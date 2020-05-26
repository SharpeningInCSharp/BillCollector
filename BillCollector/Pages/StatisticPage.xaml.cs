using DataBaseContext;
using DataBaseContext.Diagrams;
using DiagramControls;
using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BillCollector.Pages
{
	/// <summary>
	/// Логика взаимодействия для StatisticPage.xaml
	/// </summary>
	public partial class StatisticPage : Page, IDateManagmentPage
	{
		public DateTime StartDareTime { get; } = DateTime.Today;
		private readonly DateTime InitialDate = IDateManagmentPage.InitialDate;
		public PieDiagram diagram;

		private readonly Func<GoodType, DateTime, DateTime?, IEnumerable<Expence.ExpenceSelection>> dataProvider;
		private readonly SolidColorBrush[] UserBrushes = new SolidColorBrush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Purple, Brushes.Cyan, Brushes.Orange };

		public StatisticPage()
		{
			InitializeComponent();

			dataProvider = DataBase.SelectAndDistinct;

			Display();
		}

		private async void Display()
		{
			Calendar.DisplayDateStart = new DateTime(InitialDate.Year, InitialDate.Month, InitialDate.Day);

			Initialize();

			//await Task.Run(() => CrossOutEmptyDates());
			//'cause DateRange can be really big
			await Task.Run(() => ((IDateManagmentPage)this).CrossOutEmptyDatesAsync(Dispatcher, Calendar));			
		}

		//private void CrossOutEmptyDates()
		//{
		//	var avaialbleDates = DataBase.GetAvailableDates();
		//	var currentDate = DateTime.Today;
		//	while (currentDate != InitialDate)
		//	{
		//		if (avaialbleDates.Count(x => x.Year == currentDate.Year &&
		//								x.Month == currentDate.Month &&
		//								x.Day == currentDate.Day) == 0)     // if there's no such date
		//		{
		//			Dispatcher.Invoke(() => Calendar.BlackoutDates.Add(new CalendarDateRange(currentDate)));
		//		}

		//		currentDate = currentDate.AddDays(-1);
		//	}
		//}

		private void Initialize()
		{
			DateTime lastExpenceD;
			Calendar.SelectedDate = lastExpenceD = DataBase.GetLastExpenceDate();
			Calendar.DisplayDateEnd = DateTime.Today;

			var sc = new Scopes<GoodType, Expence.ExpenceSelection>(dataProvider, typeof(GoodType), lastExpenceD, null);
			diagram = new PieDiagram(sc, UserBrushes);
			Grid.SetRow(diagram, 0);
			Grid.SetColumn(diagram, 1);
			mainGrid.Children.Add(diagram);
		}

		private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			if (diagram != null)
			{
				Mouse.Capture(null);

				var dates = Calendar.SelectedDates.OrderBy(x => x.Date);
				DateTime initial = dates.First();
				DateTime? final = dates.Last();

				if (initial == final.Value)
					final = null;

				var sc = new Scopes<GoodType, Expence.ExpenceSelection>(dataProvider, typeof(GoodType), initial, final);
				diagram.LoadNew(sc);
			}
		}
	}
}

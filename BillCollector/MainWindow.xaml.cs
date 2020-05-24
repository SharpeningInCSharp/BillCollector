using System.Windows;
using System.Windows.Media;
using System;
using DiagramControls;
using DataBaseContext.Diagrams;
using DataBaseContext;
using GoodInfo;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;

namespace BillCollector
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. This's user's part!
	/// </summary>
	public partial class MainWindow : Window
	{
		public DateTime StartDareTime { get; } = DateTime.Today;
		private readonly DateTime InitialDate = new DateTime(2019, 1, 1);
		public PieDiagram diagram;

		private Func<GoodType, DateTime, DateTime?, IEnumerable<Expence.ExpenceSelection>> dataProvider;
		private readonly SolidColorBrush[] UserBrushes = new SolidColorBrush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Purple, Brushes.Cyan, Brushes.Orange };

		//SOLVE: update button
		//SOLVE: add list of bills
		//SOLVE: add button to load bill
		public MainWindow()
		{
			InitializeComponent();

			Display();
		}

		private async void Display()
		{
			dataProvider = DataBase.SelectAndDistinct;

			Calendar.DisplayDateStart = new DateTime(InitialDate.Year, InitialDate.Month, InitialDate.Day);

			Initialize();

			await Task.Run(() => CrossOutEmptyDates());
		}

		private void CrossOutEmptyDates()
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

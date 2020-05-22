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

namespace BillCollector
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. This's user's part!
	/// </summary>
	public partial class MainWindow : Window
	{
		public DateTime StartDareTime { get; } = DateTime.Today;
		public PieDiagram diagram;

		private Func<GoodType, DateTime, DateTime?, IEnumerable<Expence.ExpenceSelection>> dataProvider;
		private readonly SolidColorBrush[] UserBrushes = new SolidColorBrush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Purple, Brushes.Cyan, Brushes.Orange };

		public MainWindow()
		{
			InitializeComponent();

			Display();
		}

		private void Display()
		{
			dataProvider = DataBase.SelectAndDistinct;

			Calendar.DisplayDateStart = new DateTime(2019, 1, 1);
			Initialize();
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

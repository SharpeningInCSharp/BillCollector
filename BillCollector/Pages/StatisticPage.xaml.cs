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
using BillCollector.Pages.PageUIItems;

namespace BillCollector.Pages
{
	/// <summary>
	/// Логика взаимодействия для StatisticPage.xaml
	/// </summary>
	public partial class StatisticPage : DateManagerBasePage
	{
		public DateTime StartDareTime { get; } = DateTime.Today;
		private PieDiagram diagram;

		private readonly Func<GoodType, DateTime, DateTime?, IEnumerable<Expence.ExpenceSelection>> dataProvider;
		private readonly SolidColorBrush[] UserBrushes = new SolidColorBrush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Purple, Brushes.Cyan, Brushes.Orange };

		public StatisticPage()
		{
			InitializeComponent();

			dataProvider = DataBase.SelectAndDistinct;

			Display();
		}

		protected sealed override void Initialize()
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

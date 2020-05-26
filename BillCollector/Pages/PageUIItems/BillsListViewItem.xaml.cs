using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace BillCollector.Pages.PageUIItems
{
	/// <summary>
	/// Логика взаимодействия для BillsListViewItem.xaml
	/// </summary>
	public partial class BillsListViewItem : UserControl
	{
		public DateTime Date { get; }
		public string PathToOpen { get; }

		public BillsListViewItem(DateTime date, string pathToOpen)
		{
			InitializeComponent();
			Date = date;
			PathToOpen = pathToOpen;

			Initialize();
		}

		private async void Initialize()
		{
			
			//Tooltip
			//await Task.Run(() =>);
		}

		private void ViewDocButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(PathToOpen);
		}
	}
}

using DataBaseContext.OutputTools;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BillCollector.Pages.PageUIItems
{
	/// <summary>
	/// Логика взаимодействия для BillsListViewItem.xaml
	/// </summary>
	public partial class BillsListViewItem : UserControl
	{
		public string PathToOpen { get; }

		public BillsListViewItem(DateTime date, string pathToOpen)
		{
			InitializeComponent();
			DateTextBlock.Text = date.ToShortDateString();
			PathToOpen = pathToOpen;

			InitializeAsync();
		}

		private void Initialize()
		{
			foreach (var item in PdfManager.ReadFile(PathToOpen))
			{
				Dispatcher.Invoke(() => MainGrid.ToolTip += item);
			}
		}

		private async void InitializeAsync()
		{
			await Task.Run(() => Initialize());
		}

		private void ViewDocButton_Click(object sender, RoutedEventArgs e)
		{
			new WebBrowser().Navigate(PathToOpen);
		}
	}
}

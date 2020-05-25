using DataBaseContext;
using iText.Layout.Renderer;
using QRCoder;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DataBaseContext.OutputTools;

namespace CashRegister.AdditionalWindows
{
	/// <summary>
	/// Логика взаимодействия для ReceipWindow.xaml
	/// </summary>
	public partial class ReceipWindow : Window
	{
		private const int TimeOut = 30;
		private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

		public ReceipWindow()
		{
			InitializeComponent();
		}

		public ReceipWindow(Expence expence, string url) : this()
		{
			var items = expence.SelectAll().Select(x => new
			{
				x.Amount,
				x.Item,
				x.Price,
				x.TotalPrice,
			});

			receipOutputDataGrid.ItemsSource = items;
			TotalPriceTB.Text = expence.Sum.ToString();
			CreateQr(url);
			StartCountDownAsync();
		}

		private void StartCountDownAsync()
		{
			Task.Run(() => StartCountDown(), tokenSource.Token);
		}

		private void StartCountDown()
		{
			for (int i = TimeOut; i >= 0; i--)
			{
				Dispatcher.Invoke(() => TimeOutTextBlock.Text = i.ToString());
				Thread.Sleep(1000);
			}

			Dispatcher.Invoke(() => Close());
		}

		private void CreateQr(string url)
		{
			QrImage.Source = QrManager.CreateQr(url);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			tokenSource.Cancel();
		}
	}
}

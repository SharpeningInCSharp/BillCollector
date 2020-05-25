using System.Threading.Tasks;
using System.Windows;
using GoodInfo;
using DataBaseContext;
using Microsoft.Win32;
using DataBaseContext.OutputTools;
using DataBaseContext.InputTools;
using CashRegister.AdditionalWindows;
using System.Threading;
using System.Collections.Generic;

namespace CashRegister
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. Here's the CashRegister app logic
	/// </summary>
	public partial class MainWindow : Window
	{
		private Expence expence = new Expence();
		public IEnumerable<Expence.ExpenceSelection> Items { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			Refresh();
		}

		private void AddGoodButton_Click(object sender, RoutedEventArgs e)
		{
			var addWin = new AddGoodWindow();
			addWin.NewGoodAdded += AddWin_NewGoodAdded;
			addWin.ShowDialog();
		}

		private void AddWin_NewGoodAdded(Good obj)
		{
			expence.Add(obj);
			Refresh();
		}

		private void Refresh()
		{
			Items = expence.SelectAll();

			Dispatcher.Invoke(() =>
			{
				goodListDataGrid.ItemsSource = Items;
				TotalPriceTextBlock.Text = expence.Sum.ToString();
			}
			);

		}

		private void CreateReceip_Click(object sender, RoutedEventArgs e)
		{
			if (expence.ItemsCount == 0)
			{
				MessageBox.Show("Bill can't be empty! Add any amount of goods.");
			}
			else
			{
				expence.CreateGuid();

				//Используется Thread потому что требуется долговременное вычисление с заданием высокого приоритета
				string path = "";
				Thread outputThread = new Thread(() => OutputData(expence, out path))
				{
					Priority = ThreadPriority.Highest,
				};
				outputThread.Start();

				Task.Run(() => ShowLoadingAmination(outputThread, ref path));
			}
		}

		private void OpenReceipWin(string path)
		{
			Dispatcher.Invoke(() =>
			{
				//var receipWin = new ReceipWindow(expence, outputThread.Result);	//Doen't work yet
				var receipWin = new ReceipWindow(expence, path);
				expence = new Expence();
				receipWin.ShowDialog();
				Refresh();
			});
		}

		private void ShowLoadingAmination(Thread runningOutputThread, ref string path)
		{
			const int Timeout = 200;
			const string initialText = "Loading";
			Dispatcher.Invoke(() => LoadingTextBlock.Visibility = Visibility.Visible);
			while ((runningOutputThread.ThreadState == ThreadState.Aborted || runningOutputThread.ThreadState == ThreadState.Stopped) == false)
			{
				Dispatcher.Invoke(() => LoadingTextBlock.Text = initialText);
				Thread.Sleep(Timeout);
				Dispatcher.Invoke(() => LoadingTextBlock.Text = initialText + ".");
				Thread.Sleep(Timeout);
				Dispatcher.Invoke(() => LoadingTextBlock.Text = initialText + "..");
				Thread.Sleep(Timeout);
				Dispatcher.Invoke(() => LoadingTextBlock.Text = initialText + "...");
				Thread.Sleep(Timeout);
			}
			Dispatcher.Invoke(() => LoadingTextBlock.Visibility = Visibility.Hidden);

			OpenReceipWin(path);
		}

		private void OutputData(Expence expence, out string path)
		{
			var fileCreationTask = PdfManager.CreateAsync(this.expence);
			fileCreationTask.Wait();
			var updateTask = DataBase.AddAsync(expence);
			path = this.expence.Bill.Path;
			Task.WaitAll(new Task[] { updateTask });
		}

		private void LoadReceip_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				InitialDirectory = @"C:\Users\aleks\source\repos\BillCollector\ReceipsSamples"
			};
			var val = openFileDialog.ShowDialog();
			if (val.HasValue && val.Value)
			{
				if (openFileDialog.FileName.EndsWith(".txt"))
				{
					ReadFileAsync(openFileDialog.FileName);
				}
				else
				{
					MessageBox.Show("Please select txt file.");
				}
			}
		}

		private async void ReadFileAsync(string path)
		{
			await Task.Run(() => ReadFile(path));
		}

		private void ReadFile(string path)
		{
			expence.Clear();
			foreach (var item in ReadReceipFromTxt.Read(path))
			{
				AddWin_NewGoodAdded(item);
			}

			Refresh();
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			var logEditorWin = new LogEditorWin();
			logEditorWin.ShowDialog();
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			if (expence.ItemsCount > 0)
			{
				if (goodListDataGrid.SelectedItems.Count == 0)
				{
					var res = MessageBox.Show("All goods'll be deleted", "Warning", MessageBoxButton.YesNo);
					if (res == MessageBoxResult.Yes)
					{
						expence.Clear();
					}
				}
				else
				{
					foreach (var item in goodListDataGrid.SelectedItems)
					{
						if (item is Expence.ExpenceSelection selection)
						{
							expence.Remove(selection);
						}
					}
				}
				Refresh();
			}
		}
	}
}

using System.Threading.Tasks;
using System.Windows;
using GoodInfo;
using DataBaseContext;
using Microsoft.Win32;
using DataBaseContext.OutputTools;
using DataBaseContext.InputTools;
using CashRegister.AdditionalWindows;

namespace CashRegister
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. Here's the CashRegister app logic
	/// </summary>
	public partial class MainWindow : Window
	{
		private Expence expence = new Expence();

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
			var items = expence.SelectAll();

			Dispatcher.Invoke(() =>
			{
				goodListDataGrid.ItemsSource = items;
				TotalPriceTextBlock.Text = expence.Sum.ToString();
			}
			);
		}

		//TODO: LoadingAnimation
		private void CreateReceip_Click(object sender, RoutedEventArgs e)
		{
			if (expence.ItemsCount == 0)
			{
				MessageBox.Show("Bill can't be empty! Add any amount of goods.");
			}
			else
			{
				expence.CreateGuid();
				var outputThread = Task.Run(() => OutputData(expence));

				while (outputThread.Status != TaskStatus.RanToCompletion)
				{
					//Animation.Visibility = Visibility.Visible;
				}
				//Animation.Visibility = Visibility.Hidden;

				//var receipWin = new ReceipWindow(expence, outputThread.Result);	//Doen't work yet
				var receipWin = new ReceipWindow(expence, "Tmp");
				expence = new Expence();
				receipWin.ShowDialog();
			}
		}

		private Task<string> OutputData(Expence expence)
		{
			var fileCreationTask = ToPDFConverter.CreateAsync(this.expence);
			fileCreationTask.Wait();
			var updateTask = DataBase.AddAsync(expence);
			var fileUploadTask = CloudBillProvider.UploadAsync(this.expence.Bill.Path);     //Doen't work yet
			Task.WaitAll(new Task[] { updateTask, fileUploadTask });
			return fileUploadTask;
		}

		private void LoadReceip_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog();
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

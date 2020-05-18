using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GoodInfo;
using DataBaseContext;
using Microsoft.Win32;
using DataBaseContext.OutputTools;
using DataBaseContext.InputTools;
using System.Reflection.Metadata.Ecma335;
using CashRegister.AdditionalWindows;

namespace CashRegister
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. Here's the CashRegister app logic
	/// </summary>
	public partial class MainWindow : Window
	{
		private Expence Expence { get; set; } = new Expence();
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
			Expence.Add(obj);
			Refresh();
		}

		private void Refresh()
		{
			var items = Expence.SelectAll();

			Dispatcher.Invoke(() =>
			{
				goodListDataGrid.ItemsSource = items;
				TotalPriceTextBlock.Text = items.Sum(x => x.TotalPrice).ToString();
			}
			);
		}

		//TODO: LoadingAnimation
		private void CreateReceip_Click(object sender, RoutedEventArgs e)
		{
			if (Expence.ItemsCount == 0)
			{
				MessageBox.Show("Bill can't be empty! Add any amount of goods.");
			}
			else
			{
				Expence.CreateGuid();
				var outputThread = Task.Run(() => OutputData(Expence));

				while (outputThread.Status == TaskStatus.Running)
				{
					//Animation.Visibility = Visibility.Visible;
				}
				//Animation.Visibility = Visibility.Hidden;
				var receipWin = new ReceipWindow(Expence, outputThread.Result);
				Expence = new Expence();
				receipWin.ShowDialog();
			}
		}

		private Task<string> OutputData(Expence expence)
		{
			var fileCreationTask = ToPDFConverter.CreateAsync(Expence);
			fileCreationTask.Wait();
			var updateTask = DataBase.AddAsync(expence);
			var fileUploadTask = CloudBillProvider.UploadAsync(Expence.Bill.Path);
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
			Expence.Clear();
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
			if (goodListDataGrid.SelectedItems.Count == 0)
			{
				var res = MessageBox.Show("All goods'll be deleted", "Warning", MessageBoxButton.YesNo);
				if (res == MessageBoxResult.Yes)
				{
					Expence.Clear();
				}
			}
			else
			{
				foreach (var item in goodListDataGrid.SelectedItems)
				{
					if (item is Expence.ExpenceSelection selection)
					{
						Expence.Remove(selection);
					}
				}
			}

			Refresh();
		}
	}
}

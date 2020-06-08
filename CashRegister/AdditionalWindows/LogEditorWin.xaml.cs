using DataBaseContext;
using DataBaseContext.OutputTools;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CashRegister.AdditionalWindows
{
	/// <summary>
	/// Логика взаимодействия для LogEditorWin.xaml
	/// </summary>
	public partial class LogEditorWin : Window
	{
		public List<GoodUseFrequence> LogItems { get; set; }

		private class LogSelection
		{
			public string Name { get; set; }
			public string Type { get; set; }
		}

		public LogEditorWin()
		{
			InitializeComponent();
			OnInitialize();
		}

		//SOLVE: check type edition
		private async void OnInitialize()
		{
			await Task.Run(() => Initialize());
		}

		private void Initialize()
		{
			LogItems = ExpenceLogManager.LoadDataAsync().Result;
			Dispatcher.Invoke(() =>
			{
				LogDataGrid.ItemsSource = LogItems.Select(x => new LogSelection
				{
					Name = x.Name,
					Type = x.Type.ToString()
				}
				).ToList();
			});
		}

		private void LoadNwButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ShowDialog();

			if (DataValidation.BinaryFileExist(openFileDialog.FileName))
			{
				ExpenceLogManager.SetNewPathAsync(openFileDialog.FileName, ReplaceCheckBox.IsChecked.Value, OutputLoadRes);
			}
		}

		private void OutputLoadRes(string outputMessage)
		{
			Initialize();
			Dispatcher.Invoke(() =>
			{
				MessageBox.Show(outputMessage);
			}
			);
		}

		private void LogDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LogDataGrid.SelectedIndex != -1)
			{
				if (LogItems.Count != 0)
				{
					var lastInd = LogDataGrid.SelectedIndex;        //'course SelectuonChanged doesn't invoke when grid's been unfocused
					if (lastInd < LogItems.Count)
					{
						var logSelection = LogDataGrid.Items[lastInd] as LogSelection;
						var log = LogItems[lastInd];
						log.ChangeName(logSelection.Name);
						log.ChangeType(logSelection.Type);
					}
				}
			}
		}

		//SOLVE: unncounce goodsList to change this log item to normal one
		//SOLVE: check LoadNew
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			ExpenceLogManager.UploadLog(LogItems, (message) => MessageBox.Show(message));
			Close();
		}

		private void LogDataGrid_LostFocus(object sender, RoutedEventArgs e)
		{
			LogDataGrid_SelectionChanged(LogDataGrid, null);
		}
	}
}

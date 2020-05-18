using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataBaseContext.OutputTools;

namespace CashRegister
{
	/// <summary>
	/// Логика взаимодействия для AddGoodWindow.xaml
	/// </summary>
	public partial class AddGoodWindow : Window
	{
		public Good Good { get; private set; } = null;
		public event Action<Good> NewGoodAdded;

		private bool NameValid = false;
		private bool PriceValid = false;
		private List<GoodUseFrequence> ItemUses;

		public AddGoodWindow()
		{
			InitializeComponent();
			GoodTypeCB.ItemsSource = Enum.GetNames(typeof(GoodType));
		}

		private void GoodNameTB_LostFocus(object sender, RoutedEventArgs e)
		{
			if (GoodInfo.DataValidation.CapitalName(GoodNameTB.Text))
			{
				NameValid = true;
				ToValidView(GoodNameTB);
			}
			else
			{
				NameValid = false;
				ToInvalidView(GoodNameTB);
			}
		}

		private void PriceTB_LostFocus(object sender, RoutedEventArgs e)
		{
			if (GoodInfo.DataValidation.CheckPrice(PriceTB.Text))
			{
				PriceValid = true;
				ToValidView(PriceTB);
			}
			else
			{
				PriceValid = false;
				ToInvalidView(PriceTB);
			}
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var message = ErrorMessage();
			if (message == string.Empty)
			{
				Good = new Good((GoodType)GoodTypeCB.SelectedIndex, GoodNameTB.Text, decimal.Parse(PriceTB.Text));
				NewGoodAdded?.Invoke(Good);
				Close();
			}
			else
			{
				MessageBox.Show(ErrorMessage());
			}
		}

		/// <summary>
		/// Generate an error message according to flags
		/// </summary>
		/// <returns>Error message or OK - string.Empty</returns>
		private string ErrorMessage()
		{
			if (NameValid is false)
				return $"Wrong good name {GoodNameTB.Text}.";

			if (PriceValid is false)
				return $"Wrong price {PriceTB.Text}.";

			if (GoodTypeCB.SelectedItem is null)
				return $"Select good type.";

			return string.Empty;
		}

		private void ToInvalidView(TextBox textBox)
		{
			textBox.BorderThickness = new Thickness(2);
			textBox.BorderBrush = new SolidColorBrush(Colors.Red);
		}

		private void ToValidView(TextBox textBox)
		{
			textBox.BorderThickness = new Thickness(2);
			textBox.BorderBrush = new SolidColorBrush(Colors.Green);
		}

		private async void GoodTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TopFrequentGoodsPopup.IsOpen = false;
			var context = (string)GoodTypeCB.SelectedItem;
			var type = (GoodType)Enum.Parse(typeof(GoodType), context);
			await Task.Run(() => PackToSelectedTopDataAsync(type));
		}

		private void PackToSelectedTopDataAsync(GoodType type)
		{
			ItemUses = ExpenceLogManager.GetTopPopularAsync(type).Result;
			PackToSelectedData(ItemUses);
		}

		private void PackToSelectedData(IEnumerable<GoodUseFrequence> goodUses)
		{
			Dispatcher.Invoke(() =>
			{
				PopupSP.Children.Clear();
				foreach (var item in goodUses)
				{
					var b = new Button()
					{
						Content = item.Name,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Focusable = true,
					};
					b.Click += PopupItem_Click;
					PopupSP.Children.Add(b);
				}
			});
		}

		private void PopupItem_Click(object sender, RoutedEventArgs e)
		{
			var tmp = ((Button)sender).Content as string;
			GoodNameTB.Text = tmp;
			TopFrequentGoodsPopup.IsOpen = false;
			GoodNameTB_LostFocus(GoodNameTB, e);
		}

		private void GoodNameTB_GotFocus(object sender, RoutedEventArgs e)
		{
			TopFrequentGoodsPopup.IsOpen = true;
		}

		private void GoodNameTB_TextChanged(object sender, TextChangedEventArgs e)
		{
			var str = ((TextBox)sender).Text;
			//SOLVE: check it:null value
			PackToSelectedData(ItemUses.Where(x => x.Name.StartsWith(str, StringComparison.CurrentCultureIgnoreCase)).
										OrderByDescending(x=>x.Times));
		}

		private void PriceTB_GotFocus(object sender, RoutedEventArgs e)
		{
			TopFrequentGoodsPopup.IsOpen = false;
		}

	}
}

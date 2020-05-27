using DataBaseContext;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BillCollector.AdditionalWindows
{
	/// <summary>
	/// Логика взаимодействия для SignUpWindow.xaml
	/// </summary>
	public partial class SignUpWindow : Window
	{
		private readonly Brush headerNormalBrush = new SolidColorBrush(Color.FromArgb(255, 237, 173, 111));
		private readonly Brush headerInvalidBrush = new SolidColorBrush(Colors.Red);

		private bool passValid = false;
		private bool loginValid = false;

		public SignUpWindow()
		{
			InitializeComponent();
		}

		private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			MainGrid.Focus();
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			if (passValid && loginValid)
			{
				Close();
				DataBase.AddAsync(new User(LoginTextBox.Text, PassTextBox.Text, (output) => Dispatcher.Invoke(() => MessageBox.Show(output))));
			}
		}

		private async void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			var login = LoginTextBox.Text;
			await Task.Run(() => CheckLogin(login));
		}

		private void CheckLogin(string login)
		{
			if (DataValidation.IsLoginValid(login))
			{
				if (DataBase.UserExist(login) == null)
				{
					LoginToNormalView();
					loginValid = true;
					return;
				}

				Dispatcher.Invoke(() => MessageBox.Show("Such login already exists!"));
			}

			LoginToInvalidView();
			loginValid = false;
		}

		private void LoginToInvalidView()
		{
			Dispatcher.Invoke(() => LoginHeaderTextBlock.Foreground = headerInvalidBrush);
		}

		private void LoginToNormalView()
		{
			Dispatcher.Invoke(() => LoginHeaderTextBlock.Foreground = headerNormalBrush);
		}

		private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (LoginTextBox.Text.Length > DataValidation.MaxLoginLength)
			{
				LoginTextBox.Text = LoginTextBox.Text.Substring(0, DataValidation.MaxLoginLength);
			}
		}

		private void PassTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (LoginTextBox.Text.Length > DataValidation.MaxLoginLength)
			{
				LoginTextBox.Text = LoginTextBox.Text.Substring(0, DataValidation.MaxLoginLength);
			}
		}

		private void PassTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (DataValidation.IsPassValid(PassTextBox.Text))
			{
				PassToNormalView();
				passValid = true;
			}
			else
			{
				PassToInvalidView();
				passValid = false;
			}

		}

		private void PassToInvalidView()
		{
			PassHeaderTextBlock.Foreground = headerInvalidBrush;
		}

		private void PassToNormalView()
		{
			PassHeaderTextBlock.Foreground = headerNormalBrush;
		}
	}
}

using DataBaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BillCollector.AdditionalWindows
{
	/// <summary>
	/// Логика взаимодействия для LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		//#EDAD6F
		private readonly Brush headerNormalBrush = new SolidColorBrush(Color.FromArgb(255, 237, 173, 111));
		private readonly Brush headerInvalidBrush = new SolidColorBrush(Colors.Red);

		private User potentialUser;

		public delegate void AccessAllowedHandler(User user);
		public event AccessAllowedHandler AccessAllowed;

		public bool AllowAccess { get; private set; } = false;

		public LoginWindow()
		{
			InitializeComponent();

			KeyDown += LoginWindow_KeyDown;
		}

		private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				TryAutorize();
			}
		}

		private void PassToInvalidView()
		{
			PassHeaderTextBlock.Foreground = headerInvalidBrush;
		}

		private void PassToValidView()
		{
			PassHeaderTextBlock.Foreground = headerNormalBrush;
		}

		private void LoginToInvalidView()
		{
			LoginHeaderTextBlock.Foreground = headerInvalidBrush;
		}

		private void LoginToValidView()
		{
			LoginHeaderTextBlock.Foreground = headerNormalBrush;
		}

		private void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			potentialUser = User.UserExist(LoginTextBox.Text);
			if (potentialUser != null)
			{
				ValidLogin();
			}
			else
			{
				InvalidLogin();
			}
		}

		private void ValidLogin()
		{
			LoginToValidView();
			PasswordBox.IsEnabled = true;
		}

		private void InvalidLogin()
		{
			LoginToInvalidView();
			PasswordBox.IsEnabled = false;
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (PasswordBox.Password.Length > DataValidation.MaxPassLength)
			{
				PasswordBox.Password = PasswordBox.Password.Substring(0, DataValidation.MaxPassLength);
			}
		}

		private void TryAutorize()
		{
			if (potentialUser != null)
			{
				if (potentialUser.IsPassValid(PasswordBox.Password))
				{
					PassToValidView();
					Close();
					AllowAccess = true;
					AccessAllowed?.Invoke(potentialUser);
					return;
				}
			}

			PassToInvalidView();
		}

		private void SignUpButton_Click(object sender, RoutedEventArgs e)
		{
			//TODO: make auto authorization for new User
			var signUpWin = new SignUpWindow();
			signUpWin.ShowDialog();
		}

		private void LogInButton_Click(object sender, RoutedEventArgs e)
		{
			TryAutorize();
		}

		private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (LoginTextBox.Text.Length > DataValidation.MaxLoginLength)
			{
				LoginTextBox.Text = LoginTextBox.Text.Substring(0, DataValidation.MaxLoginLength);
			}
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			MainGrid.Focus();
		}

	}
}

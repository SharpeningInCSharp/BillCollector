﻿using System.Windows;
using System;
using DataBaseContext;
using BillCollector.AdditionalWindows;
using BillCollector.Pages;

namespace BillCollector
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. This's user's part!
	/// </summary>
	public partial class MainWindow : Window
	{
		//SOLVE: update button
		private User user;

		public MainWindow()
		{
			var loginWin = new LoginWindow();
			loginWin.AccessAllowed += InitializeUser;
			loginWin.ShowDialog();

			if (loginWin.AllowAccess == false)
			{
				Close();
				return;
			}

			InitializeComponent();

			LoadStatisticButton_Click(null, null);
		}

		private void InitializeUser(User user)
		{
			this.user = user;
			StatisticPage.SetUser(user);
			BillsPage.SetUser(user);
		}

		private void LoadStatisticButton_Click(object sender, RoutedEventArgs e)
		{
			var statUri = new Uri(@"Pages\StatisticPage.xaml", UriKind.Relative);
			PageFrame.Source = statUri;
		}

		private void LoadBillsButton_Click(object sender, RoutedEventArgs e)
		{
			var billsUri = new Uri(@"Pages\BillsPage.xaml", UriKind.Relative);
			PageFrame.Source = billsUri;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BillCollector.Pages.PageUIItems;

namespace BillCollector.Pages
{
	/// <summary>
	/// Логика взаимодействия для BillsPage.xaml
	/// </summary>
	public partial class BillsPage : DateManagerBasePage
	{
		public BillsPage()
		{
			InitializeComponent();

			Display();
		}

		protected override void Initialize()
		{
			//load bills
		}

		private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}

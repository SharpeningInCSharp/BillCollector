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

namespace BillCollector.Pages
{
	/// <summary>
	/// Логика взаимодействия для BillsPage.xaml
	/// </summary>
	public partial class BillsPage : Page, IDateManagmentPage
	{
		private readonly DateTime InitialDate = IDateManagmentPage.InitialDate;

		public BillsPage()
		{
			InitializeComponent();

			Display();
		}

		private async void Display()
		{
			Calendar.DisplayDateStart = new DateTime(InitialDate.Year, InitialDate.Month, InitialDate.Day);

			Load();

			//'cause DateRange can be really big
			await Task.Run(() => ((IDateManagmentPage)this).CrossOutEmptyDatesAsync(Dispatcher, Calendar));
		}

		private void Load()
		{
			throw new NotImplementedException();
		}
	}
}

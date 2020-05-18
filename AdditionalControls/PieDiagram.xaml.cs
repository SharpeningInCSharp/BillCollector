using DataBaseContext;
using GoodInfo;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AdditionalControls
{
	/// <summary>
	/// Логика взаимодействия для PieDiagram.xaml
	/// </summary>
	public partial class PieDiagram : UserControl
	{
		public DateTime Initial { get; }
		public DateTime? Final { get; }

		private PieDiagram()
		{
			InitializeComponent();
			InitializeLegend();
		}

		//TODO: add validation
		public PieDiagram(DateTime dateTime) : this()
		{
			if (DataBaseContext.DataValidation.IsDateValid(dateTime) == false)
				throw new ArgumentException();

			Initial = dateTime;
			Initialize(DataBase.Select(dateTime));
		}

		public PieDiagram(DateTime initial, DateTime final) : this()
		{
			if (DataBaseContext.DataValidation.IsDateValid(initial) == false || (DataBaseContext.DataValidation.IsDateValid(final) == false))
				throw new ArgumentException();

			Initial = initial;
			Final = final;
			Task.Run(() => Initialize(DataBase.Select(initial, final)));
		}

		private void Initialize(IEnumerable<Expence> data)
		{
			var totalPrice = data.Sum(x => x.Sum);

			var foods = data.Select(x => x.GetGoodsByGoodType(GoodType.Food));
			var enter = data.Select(x => x.GetGoodsByGoodType(GoodType.Entertainment));
			var house = data.Select(x => x.GetGoodsByGoodType(GoodType.House));
			var other = data.Select(x => x.GetGoodsByGoodType(GoodType.Other));
			var transport = data.Select(x => x.GetGoodsByGoodType(GoodType.Transport));
			
			InitializePiePieces();
		}

		private void InitializePiePieces()
		{
			
		}

		private void InitializeLegend()
		{
			legend.Children.Add(new PieLegendItem(Brushes.DarkGreen, GoodType.Food.ToString()));
			legend.Children.Add(new PieLegendItem(Brushes.DarkBlue, GoodType.Entertainment.ToString()));
			legend.Children.Add(new PieLegendItem(Brushes.LightSkyBlue, GoodType.House.ToString()));
			legend.Children.Add(new PieLegendItem(Brushes.LightYellow, GoodType.Medicine.ToString()));
			legend.Children.Add(new PieLegendItem(Brushes.Pink, GoodType.Other.ToString()));
			legend.Children.Add(new PieLegendItem(Brushes.MediumPurple, GoodType.Transport.ToString()));
		}
	}
}

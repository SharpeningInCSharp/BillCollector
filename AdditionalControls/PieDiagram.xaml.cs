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
using DataBaseContext.Diagrams;

namespace AdditionalControls
{
	/// <summary>
	/// Логика взаимодействия для PieDiagram.xaml
	/// </summary>
	public partial class PieDiagram : UserControl
	{

		/// <summary>
		/// List of selected items 
		/// </summary>
		public DateTime Initial { get; }
		public DateTime? Final { get; }

		private PieDiagram()
		{
			InitializeComponent();
			InitializeLegend();
		}

		//TODO: add validation
		//public PieDiagram(Scopes<GoodType> scopes) : this()
		//{
			
		//}


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

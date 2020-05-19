using System.Windows;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using System;
using AdditionalControls;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Linq;
using DataBaseContext.Diagrams;
using DataBaseContext;
using GoodInfo;

namespace BillCollector
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml. This's user's part!
	/// </summary>
	public partial class MainWindow : Window
	{
		//public System.Windows.Point Center { get; private set; } = new System.Windows.Point(170, 200);
		//private const double factor = Math.PI / 180;
		//private const double R = 50;

		private readonly SolidColorBrush[] UserBrushes = new SolidColorBrush[] {Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Chocolate, Brushes.Yellow, Brushes.Orange };

		public MainWindow()
		{
			InitializeComponent();
			//mainPanel.Children.Add(new PieDiagram())
			TempMethod();
		}

		//SOLVE: contains calendar to choose dates range; or some other UI to creareDiagrams
		private void TempMethod()
		{
			var sc = new Scopes<GoodType, Expence.ExpenceSelection>(DataBase.SelectAndDistinct, typeof(GoodType), DateTime.Today);
			var diag = new PieDiagram(sc, UserBrushes);
			mainPanel.Children.Add(diag);
		}
	}
}

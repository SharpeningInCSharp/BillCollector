using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramControls
{
	/// <summary>
	/// Логика взаимодействия для DiagramStatInfo.xaml
	/// </summary>
	public partial class DiagramStatInfo : UserControl
	{
		private Style TbStyle { get; }
		private int itemsAmount = 0;

		public string PreHeader { get; set; } = "Total sum: ";

		public string Note
		{
			get => NoteTextBlock.Text;
			set
			{
				NoteTextBlock.Visibility = Visibility.Visible;
				NoteTextBlock.Text = value;
			}
		}

		public string Header
		{
			get => headerTexBlock.Text;
			set
			{
				headerTexBlock.Visibility = Visibility.Visible;
				headerTexBlock.Text = $"{PreHeader}{value}";
			}
		}

		public DiagramStatInfo()
		{
			InitializeComponent();
			TbStyle = FindResource("TextBlockStyle") as Style;
			MainGrid.RowDefinitions.Add(new RowDefinition());
		}

		public void Add(string column1, string column2)
		{
			var column1Tb = new TextBlock()
			{
				Text = column1,
				Style = TbStyle,
				TextAlignment = TextAlignment.Left,
			};

			var column2Tb = new TextBlock()
			{
				Text = column2,
				Style = TbStyle,
				TextAlignment = TextAlignment.Right,
			};

			MainGrid.RowDefinitions.Add(new RowDefinition());

			Grid.SetColumn(column1Tb, 0);
			Grid.SetRow(column1Tb, itemsAmount);

			Grid.SetColumn(column2Tb, 1);
			Grid.SetRow(column2Tb, itemsAmount);

			MainGrid.Children.Add(column1Tb);
			MainGrid.Children.Add(column2Tb);

			itemsAmount++;
		}

		public void Clear()
		{
			headerTexBlock.Visibility = Visibility.Collapsed;
			NoteTextBlock.Visibility = Visibility.Collapsed;
			itemsAmount = 0;

			MainGrid.RowDefinitions.Clear();
			MainGrid.RowDefinitions.Add(new RowDefinition());
			Header = "";
			MainGrid.Children.Clear();
		}
	}
}

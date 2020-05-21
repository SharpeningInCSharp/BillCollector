using System.Windows;
using System.Windows.Controls;

namespace AdditionalControls
{
	/// <summary>
	/// Displays data in two columns
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

			//var rd = new RowDefinition()
			//{
			//	Height = new GridLength(30, GridUnitType.Auto),

			//};
			//MainGrid.RowDefinitions.Add(rd);
			
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

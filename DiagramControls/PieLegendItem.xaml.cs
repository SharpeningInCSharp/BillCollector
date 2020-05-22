using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagramControls
{
	/// <summary>
	/// Логика взаимодействия для PieLegendItem.xaml
	/// </summary>
	public partial class PieLegendItem : UserControl
	{
		public Brush InitialBrush { get; }

		private double InitialFontSize { get; }
		private double InitialBorderThickness { get; }
		private double InitialSideSize { get; }

		private const double SelectedFontSize = 20;
		private const int SelectedBorderThickness = 1;


		public delegate void LegendHandler(int num);
		public event LegendHandler MouseOn;
		public event LegendHandler MouseOut;
		public int Num { get; }

		public PieLegendItem(int num, Brush color, string title)
		{
			InitializeComponent();

			InitialFontSize = ItemName.FontSize;
			InitialBorderThickness = ItemColor.BorderThickness.Left;
			InitialSideSize = ItemColor.Height;

			ItemColor.Background = InitialBrush = color;
			ItemName.Text = title;
			Num = num;
		}

		private void Item_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			ToSelectedView();
			MouseOn?.Invoke(Num);
		}

		private void ToSelectedView()
		{
			ItemColor.BorderThickness = new Thickness(SelectedBorderThickness);
			ItemColor.Height = ItemColor.MaxHeight;
			ItemColor.Width = ItemColor.MaxWidth;
			ItemName.FontSize = SelectedFontSize;
		}

		private void Item_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			ToUnselectedView();
			MouseOut?.Invoke(Num);
		}

		private void ToUnselectedView()
		{
			ItemColor.BorderThickness = new Thickness(InitialBorderThickness);
			ItemColor.Height = InitialSideSize;
			ItemColor.Width = InitialSideSize;
			ItemName.FontSize = InitialFontSize;
		}
	}
}

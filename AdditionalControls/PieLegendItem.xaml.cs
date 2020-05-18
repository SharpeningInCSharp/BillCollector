using System.Windows.Controls;
using System.Windows.Media;

namespace AdditionalControls
{
	/// <summary>
	/// Provides an easy way to display color and name
	/// </summary>
	public partial class PieLegendItem : UserControl
	{
		public Brush Color { get; }
		public string Title { get; }

		public PieLegendItem(Brush color, string title)
		{
			InitializeComponent();
			Color = color;
			Title = title;
		}
	}
}

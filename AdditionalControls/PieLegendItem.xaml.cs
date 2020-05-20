using System.Windows.Controls;
using System.Windows.Media;

namespace AdditionalControls
{
	/// <summary>
	/// Provides an easy way to display color and name
	/// </summary>
	public partial class PieLegendItem : UserControl
	{
		public PieLegendItem(Brush color, string title)
		{
			InitializeComponent();
			ItemColor.Background = color;
			ItemName.Text = title;
		}

		//TODO: isMouseOn - Invoke event and select this item on diagram
	}
}

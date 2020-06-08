using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataBaseContext;
using DataBaseContext.Diagrams;
using GoodInfo;

namespace DiagramControls
{
	/// <summary>
	/// Логика взаимодействия для PieDiagram.xaml
	/// </summary>
	public partial class PieDiagram : UserControl
	{
		//TODO: make templated
		public DateTime Initial { get; }
		public DateTime? Final { get; }

		private readonly List<PiePiece> piePieces = new List<PiePiece>();
		private const int FullAngle = 360;

		public Scopes<GoodType, ExpenseSelection> Scopes { get; private set; }
		public SolidColorBrush[] UsersBrushes { get; }

		public PieDiagram(Scopes<GoodType, ExpenseSelection> scopes, SolidColorBrush[] brushes)
		{
			if (scopes.Count() < brushes.Length)
				throw new ArgumentException($"Amount of {nameof(brushes)} must be not less then amount of members in enum {scopes.EnumType.Name}");

			if (scopes is null)
				throw new ArgumentNullException($"{nameof(scopes)} was null!");

			InitializeComponent();
			UsersBrushes = brushes;
			Scopes = scopes;

			if (Scopes.IsEmpty)
			{
				//MessageBox.Show("There's no data for this period");
				return;
			}

			InitializeLegend();
			InitializePiePieces();
			ShowGeneralInfo();
		}

		public void LoadNew(Scopes<GoodType, ExpenseSelection> scopes)
		{
			if (legend.Children.Count == 0)
				InitializeLegend();

			ClearPie();

			if (scopes is null)
				throw new ArgumentNullException($"{nameof(scopes)} was null!");

			Scopes = scopes;

			if (Scopes.IsEmpty)
			{
				MessageBox.Show("There's no data for this period");
				return;
			}

			InitializePiePieces();
			ShowGeneralInfo();
		}

		private void ClearPie()
		{
			piePieces.Clear();
			PiecesGrid.Children.Clear();
			DiagramInfo.Clear();
		}

		private void InitializePiePieces()
		{
			var generalVol = Scopes.TotalSum;
			var genAngle = 0.0;
			for (int i = 0; i < Scopes.Count(); i++)
			{
				if (Scopes[i].Sum != 0)
				{
					var angle = Convert.ToDouble((Scopes[i].Sum * FullAngle) / generalVol);
					var piePiece = new PiePiece(i, angle, UsersBrushes[i]);
					piePiece.MouseIn += PiePiece_MouseIn;
					piePiece.MouseOut += PiePiece_MouseOut;
					piePiece.Rotate(genAngle);
					genAngle += angle;
					piePieces.Add(piePiece);
					PiecesGrid.Children.Add(piePiece);
				}
			}
		}

		private void PiePiece_MouseOut(PiePiece sender)
		{
			ShowGeneralInfo();

		}

		private void ShowGeneralInfo()
		{
			piePieceHeaderTextBlock.Text = "General info";

			DiagramInfo.Clear();
			DiagramInfo.Note = "Here are the most expensive items";
			DiagramInfo.Header = Scopes.TotalSum.ToString("C2");
			Scopes.OutputData((col1, col2) => DiagramInfo.Add(col1, col2));
		}

		private void PiePiece_MouseIn(PiePiece sender)
		{
			int num = sender.Num;
			var curScope = Scopes[num];
			piePieceHeaderTextBlock.Text = $"{curScope.EnumMember}";

			DiagramInfo.Clear();
			DiagramInfo.Header = $"{Scopes[num].Sum:C2} ({curScope.Ratio: #0.##%})";
			Scopes[num].OutputData((col1, col2) => DiagramInfo.Add(col1, col2));
		}

		private void InitializeLegend()
		{
			for (int i = 0; i < Scopes.EnumStringValues.Count; i++)
			{
				var legendItem = new PieLegendItem(i, UsersBrushes[i], Scopes.EnumStringValues[i]);
				legendItem.MouseOn += LegendItem_MouseOn;
				legendItem.MouseOut += LegendItem_MouseOut;
				legend.Children.Add(legendItem);
			}
		}

		private void LegendItem_MouseOut(int num)
		{
			if (num >= 0 && num < piePieces.Count)
				piePieces[num].Unselect();
		}

		private void LegendItem_MouseOn(int num)
		{
			if (num >= 0 && num < piePieces.Count)
				piePieces[num].Select();
		}
	}
}

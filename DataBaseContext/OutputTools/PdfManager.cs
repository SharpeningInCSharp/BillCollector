using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Threading.Tasks;

namespace DataBaseContext.OutputTools
{
	public static class PdfManager
	{
		private const string Header = "Receip";
		private const string PreDate = "Purchase time:";
		private const string PostHeaderInfo = "Bla-bla-bla-bla-bla-bla-bla-bla-bla-bla-bla-bla-bla";
		private const int HeaderFontSize = 24;
		private const int MainFontSize = 14;
		private const int HeaderTopMargin = 20;
		private const int TotalTextFontSize = 20;
		private const int TotalSumFontSize = 18;
		private readonly static string Separator = new string('*', 33);

		//private readonly static string DirPath = @"C:\Users\User\source\repos\BillCollector\DataBaseContext\Bills\";
		private readonly static string DirPath = @"C:\Users\aleks\Source\Repos\BillCollector\DataBaseContext\Bills\";

		public static async Task CreateAsync(Expence expence)
		{
			var path = await Task.Run(() => Create(expence));
			expence.CreateBill(path);
		}

		//RETURNS PATHs
		/// <summary>
		/// 
		/// </summary>
		/// <param name="expence"></param>
		private static string Create(Expence expence)
		{
			var filePath = System.IO.Path.Combine(DirPath, expence.IdentityGuid.ToString() + ".pdf");

			var pdfDoc = new PdfDocument(new PdfWriter(filePath));
			var doc = new Document(pdfDoc, PageSize.B7);
			///SOLVE: apply B7 - size, add Margin before Header. Find out problem with crossed sum. Out Date in format: hh.mm dd.mm.yy.
			///then figure out Separator length OR find way to fill hole line with it
			doc.Add(new Paragraph(Header).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
										.SetFontSize(HeaderFontSize)
										.SetMarginTop(HeaderTopMargin));
			doc.Add(new Paragraph(PostHeaderInfo));

			doc.Add(new Paragraph(Separator));
			foreach (var item in expence.Goods)
			{
				var itemStr = $"{item.Value} {item.Key.Name} - {item.Key.Price * item.Value}";
				doc.Add(new Paragraph(itemStr).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
											.SetFontSize(MainFontSize));
			}
			doc.Add(new Paragraph(Separator));
			
			var sumParagraph = new Paragraph().SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
			sumParagraph.Add("TOTAL:").SetFontSize(TotalTextFontSize).SetUnderline();
			sumParagraph.Add($"{expence.Sum:C2}").SetFontSize(TotalSumFontSize);
			doc.Add(sumParagraph);

			doc.Add(new Paragraph($"{PreDate} {expence.Date:hh:mm dd.mm.yyyy}")
					.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
					.SetFontSize(MainFontSize));

			doc.Close();
			return filePath;
		}

		//public static async Task ReadAsync(string path)
		//{

		//}
	}
}
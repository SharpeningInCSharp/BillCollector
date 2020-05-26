using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataBaseContext.OutputTools
{
	public static class PdfManager
	{
		private const string Header = "Receip";
		private const string PreDate = "Purchase time:";
		private const string PostHeaderInfo = "Provided by union of trading companies.";
		private const string PreSumText = "TOTAL:";

		private const int DocTopMargin = 20;
		private const int DocRightMargin = 6;
		private const int DocButtomMargin = 6;
		private const int DocLeftMargin = 4;

		private const int HeaderFontSize = 20;
		private const int MainFontSize = 12;
		private const int TotalSumFontSize = 14;

		private readonly static string Separator = new string('*', 50);

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
			doc.SetMargins(DocTopMargin, DocRightMargin, DocButtomMargin, DocLeftMargin);

			doc.Add(new Paragraph(Header).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
										.SetFontSize(HeaderFontSize));
			doc.Add(new Paragraph(PostHeaderInfo));

			doc.Add(new Paragraph(Separator));
			foreach (var item in expence.Goods)
			{
				var itemStr = $"{item.Value} {item.Key.Name} - {item.Key.Price * item.Value}";
				doc.Add(new Paragraph(itemStr).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
											.SetFontSize(MainFontSize));
			}
			doc.Add(new Paragraph(Separator));

			doc.Add(new Paragraph($"{PreSumText} {expence.Sum:C2}").SetBold()
																.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
																.SetFontSize(TotalSumFontSize));

			doc.Add(new Paragraph($"{PreDate} {expence.Date:HH:mm dd.mm.yyyy}")
					.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
					.SetFontSize(MainFontSize));

			doc.Close();
			return filePath;
		}

		public static async Task<IEnumerable<string>> ReadAsync(string path)
		{
			return await Task.Run(() => ReadFile(path));
		}

		private static IEnumerable<string> ReadFile(string path)
		{
			var pdfDoc = new PdfDocument(new PdfReader(path));

			long fileLength = pdfDoc.GetNumberOfPages();
			for (int pageNum = 1; pageNum < fileLength; pageNum++)
			{
				var page = pdfDoc.GetPage(pageNum);
				yield return PdfTextExtractor.GetTextFromPage(page);
			}
		}
	}
}
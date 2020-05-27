using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing;

namespace CashRegister.AdditionalWindows
{
	public class QrManager
	{
		private static readonly string qrPath = "QrImage.png";
		private static readonly string qrDirPath = @"C:\Users\aleks\source\repos\BillCollector\CashRegister\bin\Debug\netcoreapp3.1";
		/// <summary>
		/// Creates QR using given data and saves it
		/// </summary>
		/// <param name="data">data to be Encoded</param>
		/// <returns>QR as BitmapImage</returns>
		public static BitmapImage CreateQr(string data)
		{
			QRCodeEncoder codeEncoder = new QRCodeEncoder
			{
				QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H,
				QRCodeScale = 10
			};

			var qr = codeEncoder.Encode(data);
			SaveFile(qr);

			var bitI = new BitmapImage();
			bitI.BeginInit();
			bitI.UriSource = new Uri(qrPath, UriKind.Relative);
			bitI.CacheOption = BitmapCacheOption.OnLoad;
			bitI.EndInit();

			return bitI;
		}

		private static void SaveFile(Bitmap qr)
		{
			using var fs = new FileStream(qrPath, FileMode.Create);
			qr.Save(fs, ImageFormat.Png);
		}

		/// <summary>
		/// Decodes last qr
		/// </summary>
		/// <returns>string data of qr</returns>
		public static string DecodeQr()
		{
			var path = Path.Combine(qrDirPath, qrPath);
			var bitmap = new Bitmap(path);
			var qrImage = new QRCodeBitmapImage(bitmap);

			return new QRCodeDecoder().Decode(qrImage);

		}
	}
}

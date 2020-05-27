using QRCoder;
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
			var qR = new QRCode(new QRCodeGenerator().CreateQrCode(data, QRCodeGenerator.ECCLevel.Q));

			///не костыль! Нужно привести System.Drawing.Bitmap к Syste.Windows.Controls.Image, заумные методы привести не могут. 
			///поэтому или в ручную по пикселям копировать или сохранить и подгрузить
			using (var stream = new FileStream(qrPath, FileMode.Create))
			{
				qR.GetGraphic(50).Save(stream, ImageFormat.Png);
			}

			var bitI = new BitmapImage();
			bitI.BeginInit();
			bitI.UriSource = new Uri(qrPath, UriKind.Relative);
			bitI.CacheOption = BitmapCacheOption.OnLoad;
			bitI.EndInit();

			return bitI;
		}

		/// <summary>
		/// Decodes lasr qr
		/// </summary>
		/// <returns>string message pf qr</returns>
		public static string DecodeQr()
		{
			var path = Path.Combine(qrDirPath, qrPath);
			var bitmap = new Bitmap(path);
			var qrImage = new QRCodeBitmapImage(bitmap);

			return new QRCodeDecoder().Decode(qrImage);

		}
	}
}

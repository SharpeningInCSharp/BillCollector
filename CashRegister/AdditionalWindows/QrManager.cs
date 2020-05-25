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
	internal class QrManager
	{
		private static readonly string qrPath = "QrImage.png";

		public static BitmapImage CreateQr(string path)
		{
			var qR = new QRCode(new QRCodeGenerator().CreateQrCode(path, QRCodeGenerator.ECCLevel.Q));

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

		public static string DecodeQr()
		{
			return new QRCodeDecoder().Decode(new QRCodeBitmapImage(new Bitmap(qrPath)));
		}
	}
}

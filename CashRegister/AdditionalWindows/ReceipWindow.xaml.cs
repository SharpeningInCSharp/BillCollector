using DataBaseContext;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CashRegister.AdditionalWindows
{
	/// <summary>
	/// Логика взаимодействия для ReceipWindow.xaml
	/// </summary>
	public partial class ReceipWindow : Window
	{
		private static readonly string RelDir = @"C:\Users\User\source\repos\BillCollector\CashRegister\bin\Debug\netcoreapp3.1";
		public ReceipWindow()
		{
			InitializeComponent();
		}

		public ReceipWindow(Expence expence, string url) : this()
		{
			var items = expence.SelectAll().Select(x => new
			{
				x.Amount,
				x.Item,
				x.Price,
			});

			receipOutputDataGrid.ItemsSource = items;
			TotalPriceTB.Text = items.Sum(x => x.Price).ToString();
			CreateQr(url);
		}

		private void CreateQr(string url)
		{
			var qg = new QRCodeGenerator();
			var qR = new QRCode(qg.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q));

			///не костыль! Нужно привести System.Drawing.Bitmap к Syste.Windows.Controls.Image, заумные методы привести не могут. 
			///поэтому или в ручную по пикселям копировать или сохранить и подгрузить
			string path = "temp.png";
			path = System.IO.Path.Combine(RelDir, path);
			using (var stream = new FileStream(path, FileMode.Create))		
			{
				qR.GetGraphic(50).Save(stream, ImageFormat.Png);
			}

			var bitI = new BitmapImage();
			bitI.BeginInit();
			bitI.UriSource = new Uri(path);
			bitI.CacheOption = BitmapCacheOption.OnLoad;
			bitI.EndInit();

			QrImage.Source = bitI;
		}
	}
}

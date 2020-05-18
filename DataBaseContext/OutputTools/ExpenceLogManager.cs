using GoodInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace DataBaseContext.OutputTools
{
	public static class ExpenceLogManager
	{
		[Serializable]
		public class GoodUseFrequence : IEquatable<string>, IEquatable<GoodUseFrequence>
		{
			public string Name { get; set; }

			public int Times { get; private set; }

			public GoodType Type
			{
				get;
				private set;
			}

			internal GoodUseFrequence(GoodType goodType, string name, int times)
			{
				Name = name;
				Times = times;
				Type = goodType;
			}

			public void IncreaseTime()
			{
				Times++;
			}

			public void ChangeName(string name)
			{
				if (GoodInfo.DataValidation.CapitalName(name))
					Name = name;
			}

			public void ChangeType(string type)
			{
				if (Enum.TryParse(typeof(GoodType), type, out var result))
				{
					Type = (GoodType)result;
				}
			}

			public bool Equals(string other)
			{
				return Name.Equals(other);
			}

			public bool Equals(GoodUseFrequence other)
			{
				return Equals(other.Name);
			}
		}

		private static string LogPath { get; set; } = @"C:\Users\User\source\repos\BillCollector\DataBaseContext\OutputTools\Resources\GoodLog.dat";
		private static readonly object locker = new object();

		/// <summary>
		/// Wrapper for async method
		/// </summary>
		/// <returns>Top frequent goods</returns>
		public static async Task<List<GoodUseFrequence>> GetTopPopularAsync(GoodType goodType)
		{
			return await Task.Run(() => GetTopPopular(goodType));
		}

		/// <summary>
		/// Gets top frequent goods from file
		/// </summary>
		/// <returns>top freqent goods</returns>
		private static List<GoodUseFrequence> GetTopPopular(GoodType goodType)
		{
			var data = LoadData();
			if (data is null)
				throw new ArgumentNullException("Error occured in LoadData()");

			return data.Where(x => x.Type == goodType).OrderByDescending(x => x.Times).Take(3).ToList();
		}

		/// <summary>
		/// Wrapper for async method
		/// </summary>
		internal static async void SaveDataAsync(Good good)
		{
			await Task.Run(() => SaveData(good));
		}

		/// <summary>
		/// Saves new log data
		/// </summary>
		private static void SaveData(Good good)
		{
			var frequentGoods = LoadData();

			UpdateInfo(frequentGoods, good);
			Save(frequentGoods);
		}

		private static void Save(List<GoodUseFrequence> frequentGoods)
		{
			lock (locker)
			{
				DistincData(frequentGoods);
				using var bWriter = new FileStream(LogPath, FileMode.OpenOrCreate);
				var bFormatter = new BinaryFormatter();
				bFormatter.Serialize(bWriter, frequentGoods);
			}
		}

		private static void DistincData(List<GoodUseFrequence> frequentGoods)
		{
			for (int i = 0; i < frequentGoods.Count; i++)
			{
				for (int j = i + 1; j < frequentGoods.Count; j++)
				{
					if (frequentGoods[i].Name == frequentGoods[j].Name)
					{
						for (int t = 0; t < frequentGoods[j].Times; t++)
							frequentGoods[i].IncreaseTime();

						frequentGoods.RemoveAt(j);
						i--;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Update current list of goods' uses to be saved
		/// </summary>
		/// <param name="frequentGoods">Current list of goods' uses</param>
		private static void UpdateInfo(List<GoodUseFrequence> frequentGoods, Good good)
		{
			if (frequentGoods.Contains(good.Name))
			{
				frequentGoods.GetItem(good.Name).IncreaseTime();
			}
			else
			{
				frequentGoods.Add(new GoodUseFrequence(good.Type, good.Name, 1));
			}
		}

		public static async Task<List<GoodUseFrequence>> LoadDataAsync()
		{
			return await Task.Run(() => LoadData());
		}

		/// <summary>
		/// Load log of current list of goods' uses from file
		/// </summary>
		/// <returns></returns>
		private static List<GoodUseFrequence> LoadData()
		{
			lock (locker)
			{
				if (File.Exists(LogPath))
				{
					using var bReader = new FileStream(LogPath, FileMode.Open);

					var bFormatter = new BinaryFormatter();

					try
					{
						return (List<GoodUseFrequence>)bFormatter.Deserialize(bReader);
					}
					catch
					{
						return null;
					}

				}

				return new List<GoodUseFrequence>();
			}
		}

		public static async void SetNewPathAsync(string path, bool replace, Action<string> output)
		{
			await Task.Run(() => SetNewPath(path, replace, output));
		}

		private static void SetNewPath(string path, bool replace, Action<string> output)
		{
			var prevPath = LogPath;
			LogPath = path;

			var logInput = LoadData();
			if (logInput == null)           //meaning error occured
			{
				LogPath = prevPath;
				output?.Invoke("Error occured.");
			}
			else
			{
				if (replace == false)
				{
					LogPath = prevPath;
					var existed = LoadData();
					Save(existed.Union(logInput).ToList());
				}

				output?.Invoke("Successfull.");
			}
		}

		public static async void UploadLog(List<GoodUseFrequence> goodUses, Action<string> announce)
		{
			try
			{
				await Task.Run(() => Save(goodUses));
				announce?.Invoke("Log successfully updated.");
			}
			catch
			{
				announce?.Invoke("Error occured on log updating");
			}
		}
	}
}

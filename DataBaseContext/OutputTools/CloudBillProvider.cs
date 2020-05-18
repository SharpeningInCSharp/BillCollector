using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GFile = Google.Apis.Drive.v3.Data.File;

namespace DataBaseContext.OutputTools
{
	/// <summary>
	/// Uses to connect with GoogleDrive
	/// </summary>
	public static class CloudBillProvider
	{
		private const string ApplicationName = "BillCollector";
		//private static readonly string tokenPath = @"C:\Users\User\source\repos\BillCollector\DataBaseContext\OutputTools\Resources\token.json";
		//private static readonly string credentialsPath = @"C:\Users\User\source\repos\BillCollector\DataBaseContext\OutputTools\Resources\credentials.json";
		private static readonly string credentialsPath = @"C:\Users\User\source\repos\BillCollector\DataBaseContext\OutputTools\Resources\client_secret.json";
		private static readonly object locker = new object();

		public async static Task<string> UploadAsync(string path)
		{
			await Task.Run(() => Upload(path));
			return string.Empty;		//URL!
		}

		private static void Upload(string path)
		{
			Console.WriteLine("Authorization");
			var authorizationTask = Task.Run(() => Authorization()).Result;
			IList<GFile> res = authorizationTask.Files.List().Execute().Files;
			Console.WriteLine("Reading files");
			foreach(var item in res)
			{
				Console.WriteLine($"Name: {item.Name} Id {item.Id}");
			}
			//var createBillTask = Task.Run(() => CreateBillFile(expence));

			//Task.WhenAll(authorizationTask, createBillTask);

			//Upload(authorizationTask.Result, createBillTask.Result);
		}

		//private static string GetMimeType(string fileName)
		//{
		//	string mimeType = "application/unknown";
		//	string ext = Path.GetExtension(fileName).ToLower();

		//	Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
		//	if (regKey != null && regKey.GetValue("Content Type") != null)
		//		mimeType = regKey.GetValue("Content Type").ToString();

		//	//System.Diagnostics.Debug.WriteLine(mimeType); 
		//	return mimeType;
		//}

		private static Google.Apis.Drive.v3.Data.File Upload(DriveService driveService, string filePath)
		{
			if (File.Exists(filePath))
			{
				Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
				body.Name = Path.GetFileName(filePath);
				//body.MimeType = 
				//body.MimeType = GetMimeType(_uploadFile);
				// body.Parents = new List<string> { _parent };// UN comment if you want to upload to a folder(ID of parent folder need to be send as paramter in above method)
				byte[] byteArray = File.ReadAllBytes(filePath);
				var stream = new MemoryStream(byteArray);
				try
				{
					FilesResource.CreateMediaUpload request = driveService.Files.Create(body, stream, "");
					request.SupportsTeamDrives = true;
					request.UploadAsync();

					//request.ProgressChanged += Request_ProgressChanged;
					//request.ResponseReceived += Request_ResponseReceived;
					//request.Upload();
					return request.ResponseBody;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					return null;
				}
			}
			else
			{
				throw new ArgumentException("The file does not exist.");
			}

		}

		private static DriveService Authorization()
		{
			//Get permission
			GetCredential(out var credential);

			var service = new DriveService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			return service;
		}

		/// <summary>
		/// Gets permission on first run to access application to Google Drive
		/// </summary>
		/// <param name="credential"></param>
		private static void GetCredential(out UserCredential credential)
		{
			var scopes = new string[] { DriveService.Scope.Drive };
			lock (locker)
			{
				//var token = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				//token = Path.Combine(token, "DriveAPICredengital", "token-credentials.json");
				string credPath = "token.json";
				using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
																		scopes, Environment.UserName, CancellationToken.None,
																		new FileDataStore(credPath, true)).Result;
			}
		}

		//Create file with log-shop name...
		private static string CreateBillFile(Expence expence)
		{
			string path = "test.txt";

			using var sr = new StreamWriter(path);
			for (int i = 0; i < 10; i++)
				sr.WriteLine("Jopa");

			return path;
		}
	}
}

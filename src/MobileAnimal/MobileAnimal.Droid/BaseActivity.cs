using System;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.IO;
using MobileAnimal.Core;

namespace MobileAnimal.Droid
{
	public class BaseActivity : Activity
	{
		private const string _applicationURL = @"https://bomba.azurewebsites.net";
		private static MobileServiceClient _mobileServiceClient;
		private static MobileServiceUser _user;

		public static MobileServiceClient Client {
			get { 
				return _mobileServiceClient ?? (_mobileServiceClient = new MobileServiceClient(_applicationURL)); 
			}
		}

		public static MobileServiceUser CurrentUser {
			get { return _user; }
			set { _user = value; }
		}


		public void CreateAndShowDialog(Exception exception, String title)
		{
			CreateAndShowDialog(exception.Message, title);
		}

		public void CreateAndShowDialog(string message, string title)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetMessage(message);
			builder.SetTitle(title);
			builder.Create().Show();
		}
	}
}



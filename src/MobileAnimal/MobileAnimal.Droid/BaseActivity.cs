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

namespace MobileAnimal.Droid
{
	/// <summary>
	/// Base activity for reuse
	/// </summary>
	public class BaseActivity : Activity
	{
		#region [Attrs]

		private const string _applicationURL = @"https://bomba.azurewebsites.net";
		private static MobileServiceClient _mobileServiceClient;
		private static MobileServiceUser _user;

		#endregion

		#region [Props]

		public static MobileServiceClient Client {
			get { 
				return _mobileServiceClient ?? (_mobileServiceClient = new MobileServiceClient(_applicationURL)); 
			}
		}

		public static MobileServiceUser CurrentUser {
			get { return _user; }
			set { _user = value; }
		}

		#endregion

		#region [Methods]

		/// <summary>
		/// Creates the and show dialog.
		/// </summary>
		/// <param name="exception">Exception.</param>
		/// <param name="title">Title.</param>
		public void CreateAndShowDialog(Exception exception, String title)
		{
			CreateAndShowDialog(exception.Message, title);
		}

		/// <summary>
		/// Creates the and show dialog.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="title">Title.</param>
		public void CreateAndShowDialog(string message, string title)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetMessage(message);
			builder.SetTitle(title);
			builder.Create().Show();
		}

		#endregion
	}
}



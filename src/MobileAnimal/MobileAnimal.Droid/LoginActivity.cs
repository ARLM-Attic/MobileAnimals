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
using Android.Content;

namespace MobileAnimal.Droid
{
	[Activity(Label = "MobileAnimal", 
		MainLauncher = true, 
		Icon = "@mipmap/icon")]
	public class LoginActivity : BaseActivity
	{
		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Login);

			CurrentPlatform.Init();

			var loginButton = FindViewById<Button>(Resource.Id.loginButton);

			loginButton.Click += async (sender, e) => {
				
				if (await Authenticate()) {

					var intent = new Intent(this, typeof(MainActivity));
					intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask); 
					StartActivity(intent);
					Finish();
				
				} else {
					CreateAndShowDialog("Authentication failed", "Ops!");
				}
			};
		}

		#region [Azure methods]

		/// <summary>
		/// Authenticate user.
		/// </summary>
		private async Task<bool> Authenticate()
		{
			var success = false;
			try {
				// Sign in with Facebook login using a server-managed flow.
				CurrentUser = await Client.LoginAsync(this,
					MobileServiceAuthenticationProvider.Facebook);

				CreateAndShowDialog(string.Format("you are now logged in - {0}", CurrentUser.UserId), "Logged in!");

				success = true;
			} catch (Exception ex) {
				
				CreateAndShowDialog(ex, "Authentication failed");
			}
			return success;
		}

		#endregion
	}
}



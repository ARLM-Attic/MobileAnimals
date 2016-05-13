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
//        MainLauncher = true, 
        Icon = "@mipmap/icon")]
    public class LoginActivity : BaseActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);

            var loginButton = FindViewById<Button>(Resource.Id.loginButton);

            loginButton.Click += async (sender, e) =>
            {
				
                if (await Authenticate())
                {

                    var intent = new Intent(this, typeof(MainActivity));
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask); 
                    StartActivity(intent);
                    Finish();
				
                }
                else
                {
                    CreateAndShowDialog("Authentication failed", "Ops!");
                }
            };
        }

	
        private async Task<bool> Authenticate()
        {
            var success = false;
            try
            {
	
                CurrentUser = await Client.LoginAsync(this,
                    MobileServiceAuthenticationProvider.Facebook);
                
                success = true;

            }
            catch (Exception ex)
            {
				
                CreateAndShowDialog(ex, "Authentication failed");
            }

            return success;
        }
    }
}



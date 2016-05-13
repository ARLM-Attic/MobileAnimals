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
using Android.Net;
using Gcm.Client;

namespace MobileAnimal.Droid
{
    [Activity(Label = "MobileAnimal", 
        MainLauncher = true, 
        Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity
    {
        private const string _localDbFilename = "localanimalstore.db";

        private IMobileServiceSyncTable<Animal> _table;
        private AnimalAdapter _adapter;
        private ListView _listview;

        private static MainActivity _instance = new MainActivity();

        public static MainActivity CurrentActivity
        {
            get
            {
                return _instance;
            }
        }

        private void EnablePush()
        {
            _instance = this;
            
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);
            GcmClient.Register(this, AnimalBroadcastReceiver.senderIDs);
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
			
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            EnablePush();

            await InitLocalStoreAsync();

            var saveButton = FindViewById<Button>(Resource.Id.saveButton);
            var nameEditText = FindViewById<TextView>(Resource.Id.nameEditText);

            _listview = FindViewById<ListView>(Resource.Id.animalsListView);
            _table = Client.GetSyncTable<Animal>();
            _adapter = new AnimalAdapter(this);
            _listview.Adapter = _adapter;

            saveButton.Click += async (sender, e) =>
            {
                var animal = new Animal()
                { 
                    Name = nameEditText.Text 
                };

                try
                {
                    await _table.InsertAsync(animal); 
                    await SyncAsync(); 

                    _adapter.Add(animal);

                }
                catch (Exception ex)
                {
                    CreateAndShowDialog(ex, "Error");
                }

                nameEditText.Text = string.Empty;
            };

            OnRefreshItemsSelected();
        }

        #region [Android menu]

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_refresh)
            {
                item.SetEnabled(false);

                OnRefreshItemsSelected();

                item.SetEnabled(true);
            }
            return true;
        }

        #endregion

        #region [Azure methods]

        private async Task InitLocalStoreAsync()
        {
			
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
                              _localDbFilename);

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Animal>();

            await Client.SyncContext.InitializeAsync(store);
        }

        private bool HasInternet()
        {
            var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
            return (wifiInfo != null) && wifiInfo.IsConnected;
        }

        private async Task SyncAsync(bool pullData = false)
        {
            try
            {
                if (HasInternet())
                {
                    await Client.SyncContext.PushAsync();
				
                    if (pullData)
                    {
                        await _table.PullAsync("allAnimalItems", _table.CreateQuery());
                    }
                }

            }
            catch (Java.Net.MalformedURLException)
            {
                CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        private async void OnRefreshItemsSelected()
        {
            await SyncAsync(pullData: true); 
            await RefreshItemsFromTableAsync(); 
        }

        private async Task RefreshItemsFromTableAsync()
        {
            try
            {
                var list = await _table.ToListAsync();

                _adapter.Clear();

                foreach (Animal current in list)
                    _adapter.Add(current);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        #endregion
    }
}



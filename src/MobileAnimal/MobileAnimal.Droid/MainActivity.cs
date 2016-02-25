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

namespace MobileAnimal.Droid
{
	[Activity(Label = "MobileAnimal", 
		MainLauncher = true, 
		Icon = "@mipmap/icon")]
	public class MainActivity : BaseActivity
	{
		#region [Attrs]

		private const string _localDbFilename = "animallocalstore.db";
		private IMobileServiceSyncTable<Animal> _table;
		private AnimalAdapter _adapter;
		private ListView _listview;

		#endregion

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Main);

//			CurrentPlatform.Init();

			await InitLocalStoreAsync();

			var saveButton = FindViewById<Button>(Resource.Id.saveButton);
			var nameEditText = FindViewById<TextView>(Resource.Id.nameEditText);

			_listview = FindViewById<ListView>(Resource.Id.animalsListView);
			_table = Client.GetSyncTable<Animal>();
			_adapter = new AnimalAdapter(this);

			var listViewToDo = FindViewById<ListView>(Resource.Id.animalsListView);
			listViewToDo.Adapter = _adapter;

			saveButton.Click += async (sender, e) => {
			
				var animal = new Animal() { 
					Name = nameEditText.Text //, UserId = CurrentUser.UserId
				};

				try {
					
					await _table.InsertAsync(animal); // insert the new item into the local database
					await SyncAsync(); // send changes to the mobile service

					_adapter.Add(animal);

				} catch (Exception ex) {
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
			if (item.ItemId == Resource.Id.menu_refresh) {
				item.SetEnabled(false);

				OnRefreshItemsSelected();

				item.SetEnabled(true);
			}
			return true;
		}

		#endregion

		#region [Azure methods]

		/// <summary>
		/// Inits the local store.
		/// </summary>
		/// <returns>The local store async.</returns>
		private async Task InitLocalStoreAsync()
		{
			
			string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), 
				              _localDbFilename);

			if (!File.Exists(path)) {
				File.Create(path).Dispose();
			}

			var store = new MobileServiceSQLiteStore(path);
			store.DefineTable<Animal>();

			await Client.SyncContext.InitializeAsync(store);
		}

		/// <summary>
		/// Determines whether this device has internet.
		/// </summary>
		/// <returns><c>true</c> if this instance has internet; otherwise, <c>false</c>.</returns>
		private bool HasInternet()
		{
			var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
			return (wifiInfo != null) && wifiInfo.IsConnected;
		}

		/// <summary>
		/// Syncs the local data/azure.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="pullData">If set to <c>true</c> pull data.</param>
		private async Task SyncAsync(bool pullData = false)
		{
			try {

				if (HasInternet()) {
					await Client.SyncContext.PushAsync();
				
					if (pullData) {
						await _table.PullAsync("allAnimalItems", _table.CreateQuery());
					}
				}

			} catch (Java.Net.MalformedURLException) {
				CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
			} catch (Exception e) {
				CreateAndShowDialog(e, "Error");
			}
		}

		/// <summary>
		/// Raises the refresh items 
		/// </summary>
		private async void OnRefreshItemsSelected()
		{
			await SyncAsync(pullData: true); // get changes from the mobile service
			await RefreshItemsFromTableAsync(); // refresh view using local database
		}

		/// <summary>
		/// Refresh the list with the items in the local database
		/// </summary>
		/// <returns>The items from table async.</returns>
		private async Task RefreshItemsFromTableAsync()
		{
			try {
				// Get the items that weren't marked as completed and add them in the adapter
				var list = await _table.ToListAsync();

				_adapter.Clear();

				foreach (Animal current in list)
					_adapter.Add(current);

			} catch (Exception e) {
				CreateAndShowDialog(e, "Error");
			}
		}

		#endregion
			
	}
}



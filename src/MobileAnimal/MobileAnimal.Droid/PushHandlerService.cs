using System;
using Android.App;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Android.Content;

namespace MobileAnimal.Droid
{
	[Service]
	public class PushHandlerService : GcmServiceBase
	{
		public static string RegistrationID { get; private set; }

		public PushHandlerService()
			: base(AnimalBroadcastReceiver.senderIDs)
		{
		}

		protected override void OnRegistered(Context context, string registrationId)
		{
			System.Diagnostics.Debug.WriteLine("The device has been registered with GCM.", "Success!");

			// Get the MobileServiceClient from the current activity instance.
			MobileServiceClient client = MainActivity.Client;
			var push = client.GetPush();

			// Define a message body for GCM.
			const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

			// Define the template registration as JSON.
			JObject templates = new JObject();
			templates["genericMessage"] = new JObject {
				{ "body", templateBodyGCM }
			};

			try {
				// Make sure we run the registration on the same thread as the activity, 
				// to avoid threading errors.
				MainActivity.CurrentActivity.RunOnUiThread(

					// Register the template with Notification Hubs.
					async () => await push.RegisterAsync(registrationId, templates));

				System.Diagnostics.Debug.WriteLine(
					string.Format("Push Installation Id", push.InstallationId.ToString()));
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(
					string.Format("Error with Azure push registration: {0}", ex.Message));
			}
		}

		#region [implemented abstract members of GcmServiceBase]

		protected override void OnMessage(Context context, Intent intent)
		{
			string message = string.Empty;

			// Extract the push notification message from the intent.
			if (intent.Extras.ContainsKey("message")) {
				message = intent.Extras.Get("message").ToString();
				var title = "Novo animal:";

				// Create a notification manager to send the notification.
				var notificationManager = 
					GetSystemService(Context.NotificationService) as NotificationManager;

				// Create a new intent to show the notification in the UI. 
				PendingIntent contentIntent = 
					PendingIntent.GetActivity(context, 0, 
						new Intent(this, typeof(MainActivity)), 0);           

				// Create the notification using the builder.
				var builder = new Notification.Builder(context);
				builder.SetAutoCancel(true);
				builder.SetContentTitle(title);
				builder.SetContentText(message);
				builder.SetSmallIcon(Resource.Mipmap.Icon);
				builder.SetContentIntent(contentIntent);
				var notification = builder.Build();

				// Display the notification in the Notifications Area.
				notificationManager.Notify(1, notification);
			}
		}

		protected override void OnError(Context context, string errorId)
		{
			System.Diagnostics.Debug.WriteLine(
				string.Format("Error occurred in the notification: {0}.", errorId));
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}


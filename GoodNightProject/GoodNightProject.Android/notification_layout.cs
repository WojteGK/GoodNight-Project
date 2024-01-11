using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;

[assembly: Xamarin.Forms.Dependency(typeof(GoodNightProject.Droid.NotificationService))]
namespace GoodNightProject.Droid
{
    [Service(Enabled = true, Exported = false)]
    [System.Obsolete]
    public class NotificationService : IntentService
    {
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // Handle the creation and display of your full-screen notification here
            ShowFullScreenNotification();

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        void ShowFullScreenNotification()
        {
            // Create an Intent to launch the app when the notification is clicked
            Intent notificationIntent = new Intent(this, typeof(MainActivity)); // Replace MainActivity with your main activity
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.OneShot);

            //// Create a RemoteViews object for the custom layout
            //RemoteViews remoteViews = new RemoteViews(PackageName, Resource.Layout.EmptyXmlFile);

            //// Set text for the TextView in the custom layout
            //remoteViews.SetTextViewText(Resource.Id.notificationTitle, "Custom Notification Title");

            //// Set click action for the Button in the custom layout
            //remoteViews.SetOnClickPendingIntent(Resource.Id.notificationButton, GetDismissPendingIntent());

            // Build the custom notification
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "channel_id")
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentIntent(pendingIntent)
                //.SetCustomContentView(remoteViews)
                .SetAutoCancel(true);

            // Show the notification
            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(0, builder.Build());
        }

        protected override void OnHandleIntent(Intent intent)
        {
            throw new System.NotImplementedException();
        }

        //PendingIntent GetDismissPendingIntent()
        //{
        //    // Create an Intent for the dismiss action
        //    Intent dismissIntent = new Intent(this, typeof(DismissReceiver)); // Replace DismissReceiver with your BroadcastReceiver class
        //    dismissIntent.SetAction("dismiss_action");

        //    // Create a PendingIntent for the dismiss action
        //    return PendingIntent.GetBroadcast(this, 0, dismissIntent, PendingIntentFlags.OneShot);
        ////}

        //protected override void OnHandleIntent(Intent intent)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}

//using System;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Widget;
//using AndroidX.Core.App;

//[assembly: Xamarin.Forms.Dependency(typeof(GoodNightProject.Droid.NotificationService))]
//namespace GoodNightProject.Droid
//{
//    [Service(Enabled = true, Exported = false)]
//    public class NotificationService : Service, INotificationService
//    {
//        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
//        {
//            // Handle the creation and display of your custom notification here
//            ShowFullScreenNotification();

//            return StartCommandResult.Sticky;
//        }

//        public override IBinder OnBind(Intent intent)
//        {
//            return null;
//        }

//        public void ShowFullScreenNotification()
//        {
//            // Create an Intent to launch the app when the notification is clicked
//            Intent notificationIntent = new Intent(this, typeof(MainActivity)); // Replace MainActivity with your main activity
//            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.OneShot);

//            // Create a RemoteViews object for the custom layout
//            RemoteViews remoteViews = new RemoteViews(PackageName, Resource.Layout.notification_layout);

//            // Build the full-screen notification
//            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "channel_id")
//                .SetSmallIcon(Resource.Drawable.icon)
//                .SetContentIntent(pendingIntent)
//                .SetCustomContentView(remoteViews)
//                .SetAutoCancel(true)
//                .SetFullScreenIntent(pendingIntent, true); 

//            // Show the notification
//            NotificationManagerCompat notificationManager = NotificationManagerCompat.From(this);
//            notificationManager.Notify(0, builder.Build());
//        }

//    }
//}

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoodNightProject.Droid;
using System.Threading.Tasks;
using Android.Icu.Util;
using Android.Provider;
using Android.Media;

[assembly: Xamarin.Forms.Dependency(typeof(GoodNightProject.Droid.AlarmService))]
namespace GoodNightProject.Droid
{
    public class AlarmService : IAlarmService
    {
        public IBinder OnBind(Intent intent)
        {
            return null;
        }
        public void SetAlarm(int hour, int minute)
        {
            Intent alarmIntent = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, alarmIntent, PendingIntentFlags.Immutable);

            AlarmManager alarmManager = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
            Calendar calendar = Calendar.Instance;
            calendar.Set(CalendarField.HourOfDay, hour);
            calendar.Set(CalendarField.Minute, minute);
            calendar.Set(CalendarField.Second, 0);
            calendar.Set(CalendarField.Millisecond, 0);
            if (calendar.TimeInMillis < Java.Lang.JavaSystem.CurrentTimeMillis())
            {
                calendar.Add(CalendarField.DayOfMonth, 1);
            }
            alarmManager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
        }
        public void CancelAlarm()
        {
            Intent alarmIntent = new Intent(Android.App.Application.Context, typeof(AlarmReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, 0, alarmIntent, PendingIntentFlags.Immutable);
            AlarmManager alarmManager = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent);
        }
    }
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        private const string ACTION_STOP = "stop_action";
        private static bool isPlaying = false; 
        private static MediaPlayer player;
        public override void OnReceive(Context context, Intent intent)
        {
            var notificationManager = NotificationManager.FromContext(context);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = "channel_id";
                var channelName = "channel_name";
                var importance = NotificationImportance.High;
                var channel = new NotificationChannel(channelId, channelName, importance);
                notificationManager.CreateNotificationChannel(channel);
            }
            var stopAlarmIntent = new Intent(context, typeof(AlarmReceiver));
            stopAlarmIntent.SetAction(ACTION_STOP);
            var stopAlarmPendingIntent = PendingIntent.GetBroadcast(context, 0, stopAlarmIntent, PendingIntentFlags.Immutable);

            var notificationBuilder = new Notification.Builder(context, "channel_id")
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("Alarm")
                .SetContentText("Czas na coś!")
                .SetAutoCancel(true)
                .AddAction(new Notification.Action(Resource.Drawable.icon_feed, "Stop", stopAlarmPendingIntent));


            var notification = notificationBuilder.Build();
            notificationManager.Notify(0, notification);
            if (intent.Action == ACTION_STOP)
            {
                if (isPlaying && player != null)
                {
                    player.Stop();
                    player.Release();
                    player = null; 
                    isPlaying = false; 
                }
            }
            else
            {
                player = MediaPlayer.Create(context, Resource.Raw.sound);
                player.Start();
                isPlaying = true;
            }

        }
    }
}
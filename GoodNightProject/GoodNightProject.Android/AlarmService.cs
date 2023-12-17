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
        public override void OnReceive(Context context, Intent intent)
        {
            var notificationManager = NotificationManager.FromContext(context);
            var notificationBuilder = new Notification.Builder(context, "channel_id")
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("Alarm")
                .SetContentText("Czas na coś!")
                .SetAutoCancel(true);
                

            var notification = notificationBuilder.Build();
            notificationManager.Notify(0, notification);

            MediaPlayer player = MediaPlayer.Create(context, Resource.Raw.sound);
            player.Start();
        }
    }
    
}
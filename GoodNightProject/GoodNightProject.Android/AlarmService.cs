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
using Xamarin.Forms;
using static Android.Widget.RemoteViews;
using AndroidX.Core.App;
using Android.Util;
using GoodNightProject.Views;

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
        public void CancelMedia()
        {
            if (AlarmReceiver.Player != null)
            {
                AlarmReceiver.Player.Stop();
                AlarmReceiver.Player.Release();
                AlarmReceiver.Player = null;
            }
        }

    }
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        private static MediaPlayer player;
        public static MediaPlayer Player
        {
            get
            {
                if (player == null)
                {
                    player = MediaPlayer.Create(Android.App.Application.Context, Resource.Raw.sound);
                }
                return player;
            }
            set
            {
                player = value;
            }
        }
        public override void OnReceive(Context context, Intent intent)
        {
            var notificationManager = NotificationManager.FromContext(context);
            Intent intentAcitiviti = new Intent(context, typeof(AlarmActionReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, intentAcitiviti, PendingIntentFlags.Mutable);

            // Utworzenie akcji przycisku
            Notification.Action action = new Notification.Action.Builder(Resource.Drawable.abc_cab_background_top_mtrl_alpha, "Wyłącz Alarm", pendingIntent).Build();
            var notificationBuilder = new Notification.Builder(context, "channel_id")
                .SetSmallIcon(Resource.Drawable.icon_feed)
                .SetContentTitle("GoodNight App")
                .SetContentText("Wyłącz alarm")
                .SetAutoCancel(true)
                .AddAction(action);

            int notificationId = 0; // Unikalny identyfikator powiadomienia
            NotificationManager notificationManagerX = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManagerX.Notify(notificationId, notificationBuilder.Build());

            player = MediaPlayer.Create(context, Resource.Raw.sound);
            player.Start();
            

            Calendar calendar = Calendar.Instance;
            calendar.Add(CalendarField.DayOfMonth, 1);

            Intent alarmIntent = new Intent(context, typeof(AlarmReceiver));
            PendingIntent pendingIntentX = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.Immutable);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntentX);
        }

        [BroadcastReceiver(Enabled = true)]
        public class AlarmActionReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                if (AlarmReceiver.Player != null)
                {
                    AlarmReceiver.Player.Stop();
                    AlarmReceiver.Player.Release();
                    AlarmReceiver.Player = null;

                    // Wyłączenie powiadomienia po kliknięciu przycisku "Wyłącz Alarm"
                    int notificationId = 0; // Ten sam identyfikator, który został użyty do wywołania Powiadomienia
                    NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                    notificationManager.Cancel(notificationId);
                }
            }
        }

    }

}

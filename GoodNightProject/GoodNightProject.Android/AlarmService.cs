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
            MediaPlayer player = AlarmReceiver.Player;
            if (player != null && player.IsPlaying)
            {
                player.Stop();
                player.Release();
                player = null;
            }
        }

    }
    [BroadcastReceiver(Enabled = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        private static MediaPlayer player;

        public static MediaPlayer Player //getter dla CancelMedia()
        {
            get
            {
                if (player == null)
                {
                    player = MediaPlayer.Create(Android.App.Application.Context, Resource.Raw.sound);
                }
                return player;
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                player = MediaPlayer.Create(context, Resource.Raw.sound); //Gra muzyka
                player.Start();

                Intent actionIntent = new Intent(context, typeof(AlarmActionReceiver));
                actionIntent.PutExtra("ACTION", "CANCEL_ALARM_MEDIA");

                PendingIntent actionPendingIntent = PendingIntent.GetBroadcast(context, 0, actionIntent, PendingIntentFlags.Immutable);

                Intent turnOffIntent = new Intent(context, typeof(AlarmActionReceiver));
                turnOffIntent.PutExtra("ACTION", "TURN_OFF_ALARM");
                PendingIntent turnOffPendingIntent = PendingIntent.GetBroadcast(context, 0, turnOffIntent, PendingIntentFlags.Immutable);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(context, "channel_id")
                    .SetContentTitle("GoodNight APP")
                    .SetContentText("Kliknij aby wyłączyć.")
                    .SetAutoCancel(true)
                    .SetSmallIcon(Android.Resource.Color.Black)
                    .AddAction(Android.Resource.Drawable.IcMenuCloseClearCancel, "Turn Off", turnOffPendingIntent)  // Use a built-in Android resource
                    .SetPriority(NotificationCompat.PriorityHigh)  // Set the priority to high for Heads-Up notification
                    .SetDefaults(NotificationCompat.DefaultAll);   // Set defaults for sound, vibration, etc.

                NotificationManagerCompat notificationManager = NotificationManagerCompat.From(context);
                notificationManager.Notify(0, builder.Build());

                Calendar calendar = Calendar.Instance;
                calendar.Add(CalendarField.DayOfMonth, 1);

                AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
                PendingIntent alarmPendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);
                alarmManager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, alarmPendingIntent);
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Log.Error("AlarmReceiver", $"Error in OnReceive: {ex.Message}");
            }
        }
    }

    [BroadcastReceiver(Enabled = true)]
    public class AlarmActionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                string action = intent.GetStringExtra("ACTION");

                if (action == "CANCEL_ALARM_MEDIA")
                {
                    var alarmService = DependencyService.Get<IAlarmService>();
                    alarmService?.CancelAlarm();
                    alarmService?.CancelMedia();
                }
            }
            catch (Exception ex)
            {
                Log.Error("AlarmActionReceiver", $"Error in OnReceive: {ex.Message}");
            }
        }
    }

}

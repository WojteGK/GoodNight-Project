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

[assembly: Xamarin.Forms.Dependency(typeof(GoodNightProject.Droid.AlarmService))]
namespace GoodNightProject.Droid
{
    public class AlarmService : IAlarmService
    {
        public Task SetAlarm(int hour, int minute)
        {

            Intent intent = new Intent(AlarmClock.ActionSetAlarm);
            intent.PutExtra(AlarmClock.ExtraHour, hour);
            intent.PutExtra(AlarmClock.ExtraMinutes, minute);
            intent.PutExtra(AlarmClock.ExtraSkipUi, true);
            intent.AddFlags(ActivityFlags.NewTask);
            intent.PutExtra(AlarmClock.ExtraMessage, "Good Night");
            intent.PutExtra(AlarmClock.ExtraVibrate, true);
            
            Android.App.Application.Context.StartActivity(intent);
            return Task.CompletedTask;
        }
        public Task CancelAlarm()
        {
            //cancel alarm here with no open alarm clock
            Intent intent = new Intent(AlarmClock.ActionDismissAlarm);
            intent.AddFlags(ActivityFlags.NewTask);
            intent.PutExtra(AlarmClock.ExtraSkipUi, true);
            Android.App.Application.Context.StartActivity(intent);
            return Task.CompletedTask;
        }
    }
}
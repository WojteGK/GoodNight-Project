using Foundation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

[assembly: Xamarin.Forms.Dependency(typeof(GoodNightProject.iOS.AlarmServiceiOS))]
namespace GoodNightProject.iOS
{
    internal class AlarmServiceiOS : IAlarmService
    {
        public Task CancelAlarm()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
            return Task.CompletedTask;
        }

        public Task SetAlarm(int hour, int minute)
        {
            // set me alarm for hour and minute
            UNUserNotificationCenter center = UNUserNotificationCenter.Current;
            center.RequestAuthorization(UNAuthorizationOptions.Alert, (bool success, NSError error) =>
            {
                if (success)
                {
                    // Schedule notification
                    var content = new UNMutableNotificationContent();
                    content.Title = "Alarm";
                    content.Body = "Alarm";
                    content.Sound = UNNotificationSound.Default;
                    var trigger = UNCalendarNotificationTrigger.CreateTrigger(new NSDateComponents
                    {
                        Hour = hour,
                        Minute = minute,
                        Second = 0
                    }, false);
                    var request = UNNotificationRequest.FromIdentifier("Alarm", content, trigger);
                    center.AddNotificationRequest(request, (NSError obj) =>
                    {
                        if (obj != null)
                        {
                            Console.WriteLine("Error: {0}", obj);
                        }
                    });
                }
                else
                {
                    Console.WriteLine("Error: {0}", error);
                }
            });
            return Task.CompletedTask;
        }
    }
}
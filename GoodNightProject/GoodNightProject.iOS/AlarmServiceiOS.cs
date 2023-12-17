
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
        public void CancelAlarm()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
        }

        public void SetAlarm(int hour, int minute)
        {
            var content = new UNMutableNotificationContent();
            content.Title = "Alarm";
            content.Body = "Alarm";
            content.Sound = UNNotificationSound.GetSound("sound.mp3");
            content.CategoryIdentifier = "Delete";

            var calendar = new NSCalendar(NSCalendarType.Gregorian);
            var components = new NSDateComponents();
            components.Hour = hour;
            components.Minute = minute;
            var trigger = UNCalendarNotificationTrigger.CreateTrigger(components, false);

            var requestID = "Alarm";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);
            
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    // Do something with error... ????????????
                }
            });


        }
    }
}

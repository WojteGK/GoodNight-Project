using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.LocalNotification;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;

namespace GoodNightProject.Views
{
    public partial class AboutPage : ContentPage
    {
        List<TimeSpan> times = new List<TimeSpan>();
        TimeSpan selectedTime;
        public AboutPage()
        {
            InitializeComponent();
            timePicker.Unfocused += (sender, e) =>
            {
                selectedTime = timePicker.Time;

                if (selectedTime.ToString().Substring(0) == "0")
                    time.Text = selectedTime.ToString("h\\:mm");
                else
                    time.Text = selectedTime.ToString("hh\\:mm");
                Preferences.Set("time", time.Text.ToString());
            };
        }
        void SetAlarmButton_OnClick(object sender, EventArgs e)
        {
            timePicker.Focus();
        }
        async void Test(object sender, EventArgs e)
        {
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            DateTime alarmTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedTime.Hours, selectedTime.Minutes, 0);
            if(alarmTime < DateTime.Now)
                alarmTime = alarmTime.AddDays(1); 

            var notification = new NotificationRequest
            {
                
                NotificationId = 100,
                Title = "Test",
                Description = "Test Description",
                CategoryType = NotificationCategoryType.Status,
                Schedule =
                {
                     RepeatType = NotificationRepeat.Daily,
                     NotifyTime = alarmTime
                },
                Android =
                {
                    Priority = Plugin.LocalNotification.AndroidOption.AndroidPriority.Max,
                    VisibilityType = Plugin.LocalNotification.AndroidOption.AndroidVisibilityType.Public,
                },
                Sound = DeviceInfo.Platform == DevicePlatform.Android ? "sound" : "sound.mp3",
            };
            await LocalNotificationCenter.Current.Show(notification);
        } 
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Preferences.Get("time", "") != "")
            {
                time.Text = Preferences.Get("time", string.Empty);
            }
        }
    }
}
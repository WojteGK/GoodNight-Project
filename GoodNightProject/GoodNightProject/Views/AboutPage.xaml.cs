using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.LocalNotification;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace GoodNightProject.Views
{
    public partial class AboutPage : ContentPage
    {
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
                alarmTime = alarmTime.AddDays(1); /// Jesli mamy robic to w liscie to nie bedzie nam to potrzebne

            var notification = new NotificationRequest
            {
                
                NotificationId = 100,
                Title = "Test",
                Description = "Test Description",
                CategoryType = NotificationCategoryType.Status,
                Schedule =
                {
                     RepeatType = NotificationRepeat.Daily,
                     /// tutaj trzeba dodac metode ktora bedzie nam dodawac te godziny o ktorych ma sie budzic uzytkownik (czyli dodanie nowych powiadomien z dzwiekiem)
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
    }
}
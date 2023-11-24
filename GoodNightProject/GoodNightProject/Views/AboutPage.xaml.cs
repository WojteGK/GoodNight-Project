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
        bool isAlarmSet = false;
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
                SetAndCancelAlarm();
            };

        }
        private async void SetAndCancelAlarm()
        {
            IAlarmService alarmService = DependencyService.Get<IAlarmService>();
            if (isAlarmSet == false)
            {
                await alarmService.SetAlarm(selectedTime.Hours, selectedTime.Minutes);
                isAlarmSet = true;
                SetAndCancel.Text = "Anuluj Alarm";
            }
            else
            {
                await alarmService.CancelAlarm();
                isAlarmSet = false;
                SetAndCancel.Text = "Ustaw Alarm";
            }
        }
        private void SetAlarmButton_OnClick(object sender, EventArgs e)
        {
            if (isAlarmSet == false)
            {
                timePicker.Focus();
            }
            else
            {
                SetAndCancelAlarm();
            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Preferences.ContainsKey("time"))
            {
                time.Text = Preferences.Get("time", "00:00");
                timePicker.Time = TimeSpan.Parse(Preferences.Get("timeSpan", "00:00"));
                isAlarmSet = bool.Parse(Preferences.Get("isAlarmSet", "false"));
                SetAndCancel.Text = Preferences.Get("SetOrCancel", "Ustaw Alarm");
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Preferences.Set("time", time.Text.ToString());
            Preferences.Set("timeSpan", timePicker.Time.ToString());
            Preferences.Set("isAlarmSet", isAlarmSet.ToString());
            Preferences.Set("SetOrCancel", SetAndCancel.Text.ToString());
        }
    }
}
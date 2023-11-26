using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.LocalNotification;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

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
            timePicker.Unfocused += (sender, e) => // przypisywanie godziny z timepickera do labela
            {
                selectedTime = timePicker.Time;

                if (selectedTime.ToString().Substring(0) == "0")
                    time.Text = selectedTime.ToString("h\\:mm");
                else
                    time.Text = selectedTime.ToString("hh\\:mm");
                Preferences.Set("TimeText", time.Text);
                Preferences.Set("selectedTime", selectedTime.ToString());
                SetAndCancelAlarm();
            };
        }
        private async void SetAndCancelAlarm() // Tworzenie alarmu po przez interefs lub anulowanie go / zapisywanie danych do pamięci telefonu
        {
            IAlarmService alarmService = DependencyService.Get<IAlarmService>();
            if (isAlarmSet == false)
            {
                
                alarmService.SetAlarm(selectedTime.Hours, selectedTime.Minutes);
                isAlarmSet = true;
                SetAndCancel.Text = "Anuluj Alarm";
            }
            else
            {
                alarmService.CancelAlarm();
                isAlarmSet = false;
                SetAndCancel.Text = "Ustaw Alarm";
            }
            Preferences.Set("isAlarmSet", isAlarmSet.ToString());
            Preferences.Set("setAndcancel", SetAndCancel.Text);
        }
        private void SetAlarmButton_OnClick(object sender, EventArgs e) // Ustawienie godziny alarmu lub anulowanie alarmu -> zależnie od tego czy alarm jest ustawiony czy nie.
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
        protected override void OnAppearing() // Wczytywanie danych z pamięci telefonu
        {
            base.OnAppearing();
            isAlarmSet = bool.Parse(Preferences.Get("isAlarmSet", "false"));
            SetAndCancel.Text = Preferences.Get("setAndcancel", "Ustaw Alarm");
            time.Text = Preferences.Get("TimeText", "00:00");
            selectedTime = TimeSpan.Parse(Preferences.Get("selectedTime", "00:00"));
        }

    }
}
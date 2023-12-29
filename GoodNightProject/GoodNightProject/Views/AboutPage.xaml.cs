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
using GoodNightProject.Droid;

namespace GoodNightProject.Views
{
    public partial class AboutPage : ContentPage
    {
        List<TimeSpan> times = new List<TimeSpan>
        {
            new TimeSpan(0, 0, 0),
        };
        TimeSpan selectedTime;
        bool isAlarmSet = false;
        public AboutPage()
        {
            InitializeComponent();
            timePicker.Unfocused += (sender, e) => // przypisywanie godziny z timepickera do labela
            {
                times.Add(timePicker.Time);
                selectedTime = timePicker.Time;
                if (selectedTime.ToString().Substring(0) == "0")
                    time.Text = selectedTime.ToString("h\\:mm");
                else
                    time.Text = selectedTime.ToString("hh\\:mm");
            };
            
        }
        private async void SetAndCancelAlarm() // Tworzenie alarmu po przez interefs lub anulowanie go / zapisywanie danych do pamięci telefonu
        {
            IAlarmService alarmService = DependencyService.Get<IAlarmService>();

            if (isAlarmSet == false)
            {
                var godzina = algorithm(selectedTime.Hours, selectedTime.Minutes);
                alarmService.SetAlarm(godzina.Item1, godzina.Item2);
                isAlarmSet = true;
                SetAndCancel.Text = "Anuluj Alarm";
            }
            else
            {
                alarmService.CancelAlarm();
                isAlarmSet = false;
                SetAndCancel.Text = "Ustaw Alarm";
            }
        }
        private void SetAlarmButton_OnClick(object sender, EventArgs e) // Ustawienie godziny alarmu lub anulowanie alarmu -> zależnie od tego czy alarm jest ustawiony czy nie.
        {
            SetAndCancelAlarm();
        }
        private void ChangeTimeButton_OnClick(object sender, EventArgs e) // Zmiana czasu alarmu
        {
            // Klikniecie guzika otworzy nam liste z godzinami, ktore zostaly ustawione
        }
        private void AddNewTimeButton_OnClick(object sender, EventArgs e)
        {
            timePicker.Focus();
        }
        protected override void OnAppearing() // Wczytywanie danych z pamięci telefonu
        {
            base.OnAppearing();
            isAlarmSet = bool.Parse(Preferences.Get("isAlarmSet", "false"));
            SetAndCancel.Text = Preferences.Get("setAndcancel", "Ustaw Alarm");
            time.Text = Preferences.Get("TimeText", "00:00");
            selectedTime = TimeSpan.Parse(Preferences.Get("selectedTime", "00:00"));
            string json = Preferences.Get("times", "[]");
            times = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeSpan>>(json);
        }
        protected override void OnDisappearing() // Zapisywanie danych z pamieci telefonu
        {
            base.OnDisappearing();
            Preferences.Set("isAlarmSet", isAlarmSet.ToString());
            Preferences.Set("setAndcancel", SetAndCancel.Text);
            Preferences.Set("TimeText", time.Text);
            Preferences.Set("selectedTime", selectedTime.ToString());
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(times);
            Preferences.Set("times", json);
        }
        public (int, int) algorithm(int hour, int minute)
        {
            DateTime teraz = DateTime.Now; // Pobierz aktualną godzinę i minutę

            DateTime podanaData = new DateTime(teraz.Year, teraz.Month, teraz.Day, hour, minute, 0); // RObie obiekt DateTime dla podanej godziny i minuty

            TimeSpan roznicaCzasu = podanaData - teraz; // Obliczam różnicę czasu między podaną godziną a teraźniejszą godziną

            int iloscIteracji = (int)(roznicaCzasu.TotalMinutes / 1.5); // Dziele różnicę czasu przez 1,5 minut

            DateTime najblizszyCzas = teraz; // Dodaje 1,5 minutę tyle razy, ile wynosi ilość iteracji
            for (int i = 0; i < iloscIteracji; i++)
            {
                najblizszyCzas = najblizszyCzas.AddMinutes(1.5);
            }
            return (najblizszyCzas.Hour, najblizszyCzas.Minute);
        }
    }
}
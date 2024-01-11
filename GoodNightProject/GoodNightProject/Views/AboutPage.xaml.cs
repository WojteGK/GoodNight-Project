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
using System.Linq;
using System.Numerics;


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

            timePicker.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == Xamarin.Forms.TimePicker.TimeProperty.PropertyName)
                {
                    if (!times.Any(x => x.Hours == timePicker.Time.Hours && x.Minutes == timePicker.Time.Minutes))
                    {
                        times.Add(timePicker.Time);
                        if (isAlarmSet == false)
                        {
                            selectedTime = timePicker.Time;
                            if (selectedTime.ToString().Substring(0) == "0")
                                time.Text = selectedTime.ToString("h\\:mm");
                            else
                                time.Text = selectedTime.ToString("hh\\:mm");
                        }
                        Preferences.Set("TimeText", time.Text);
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(times);
                        Preferences.Set("times", json);
                    }
                    else if (times.Any(x => x.Hours == timePicker.Time.Hours && x.Minutes == timePicker.Time.Minutes))
                    {
                        DisplayAlert("Błąd", "Podana godzina już istnieje", "OK");
                    }
                }
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
                time.IsEnabled = false;
                SetAndCancel.Text = "Anuluj Alarm";
            }
            else
            {
                alarmService.CancelAlarm();
                alarmService.CancelMedia();
                isAlarmSet = false;
                time.IsEnabled = true;
                SetAndCancel.Text = "Włącz Alarm";
            }
            Preferences.Set("isAlarmSet", isAlarmSet.ToString());
            Preferences.Set("setAndcancel", SetAndCancel.Text);
        }
        private void SetAlarmButton_OnClick(object sender, EventArgs e) // Ustawienie godziny alarmu lub anulowanie alarmu -> zależnie od tego czy alarm jest ustawiony czy nie.
        {
            SetAndCancelAlarm();
        }
        private async void ChangeTimeButton_OnClick(object sender, EventArgs e) // Zmiana czasu alarmu
        {
            // Tworzymy listę guzików
            var buttons = new List<string>();
            foreach (var time in times)
            {
                if (time.ToString().Substring(0) == "0")
                    buttons.Add(time.ToString("h\\:mm"));
                else
                    buttons.Add(time.ToString("hh\\:mm"));
            }

            // Tworzymy okno dialogowe z listą guzików
            var stackLayout = new StackLayout();

            foreach (var buttonText in buttons)
            {
                var button = new Button 
                { 
                    Text = buttonText 
                };
                var binButton = new Button
                {
                    Text = "Kosz"
                };

                var horizontalLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { button, binButton }
                };

                stackLayout.Children.Add(horizontalLayout);

                binButton.Clicked += async (s, args) =>
                {
                    bool answer = await DisplayAlert("Potwierdzenie", $"Czy NA PEWNO usunąć tę godzinę: {buttonText}?", "Tak", "Anuluj");
                    if (answer)
                    {
                        TimeSpan timeSpan = TimeSpan.Parse(buttonText);
                        times = times.Where(x => x != timeSpan).ToList();
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(times);
                        Preferences.Set("times", json);
                        buttons.Remove(buttonText);
                        stackLayout.Children.Remove(horizontalLayout);
                    }
                };

                button.Clicked += async (s, args) =>
                {
                    var selectedButton = await DisplayActionSheet("Wybierz guzik", "Anuluj", null, buttons.ToArray());

                    if (!string.IsNullOrEmpty(selectedButton))
                    {
                        await DisplayAlert("Wybrano", $"Kliknięto guzik: {selectedButton}", "OK");
                    }
                };
            }
            var page = new ContentPage
            {
                Content = new Xamarin.Forms.ScrollView
                {
                    Content = stackLayout
                }
            };
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(page);
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
            if (isAlarmSet == true)
            {
                time.IsEnabled = false;
            }
        }
        public (int, int) algorithm(int hour, int minute)
        {
            DateTime teraz = DateTime.Now; // Pobierz aktualną godzinę i minutę

            DateTime podanaData = new DateTime(teraz.Year, teraz.Month, teraz.Day, hour, minute, 0); // RObie obiekt DateTime dla podanej godziny i minuty

            if(podanaData < teraz) // Jeżeli podana godzina jest mniejsza od teraźniejszej godziny
            {
                podanaData = podanaData.AddDays(1); // Dodaj 1 dzień do podanej godziny
            }

            TimeSpan roznicaCzasu = podanaData - teraz; // Obliczam różnicę czasu między podaną godziną a teraźniejszą godziną

            int iloscIteracji = (int)(roznicaCzasu.TotalMinutes / 1.5); // Dziele różnicę czasu przez 1,5 minut

            DateTime najblizszyCzas = teraz; // Dodaje 1,5 minutę tyle razy, ile wynosi ilość iteracji
            for (int i = 0; i < iloscIteracji; i++)
            {
                najblizszyCzas = najblizszyCzas.AddMinutes(1.5);
            }

            if (podanaData < teraz.AddMinutes(1.5))
            {
                return (podanaData.Hour, podanaData.Minute);
            }
            return (najblizszyCzas.Hour, najblizszyCzas.Minute);
        }
    }
}
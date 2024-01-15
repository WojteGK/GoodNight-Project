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
using System.Threading.Tasks;
using GoodNightProject.Views;


namespace GoodNightProject.Views
{
    public partial class AboutPage : ContentPage
    {
        DateTime dayWhenAlarmGoes;
        List<bool> recommendationList = new List<bool> { };
        List<TimeSpan> times = new List<TimeSpan> { };
        public TimeSpan selectedTime;
        bool isAlarmSet = false;
        bool firstTimeAppUsing = true;
        bool aboutPageSetPrefereces = true;
        public int timeToFallAsleep = 25; // in minutes
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
            //INotificationService notificationService = DependencyService.Get<INotificationService>();
            if (isAlarmSet == false)
            {
                var godzina = Algorithm(selectedTime.Hours, selectedTime.Minutes);
                alarmService.SetAlarm(godzina.Item1, godzina.Item2);
                dayWhenAlarmGoes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, godzina.Item1, godzina.Item2, 0);

                if (dayWhenAlarmGoes < DateTime.Now)
                {
                    dayWhenAlarmGoes = dayWhenAlarmGoes.AddDays(1);
                }
                Preferences.Set("dayWhenAlarmGoes", dayWhenAlarmGoes.ToString());
                isAlarmSet = true;
                time.IsEnabled = false;
                SetAndCancel.Text = "Anuluj Alarm";

                Timer();
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
            TimeSpan first = TimeSpan.Parse(time.Text);
            if (firstTimeAppUsing)
            {
                times.Add(first);
                selectedTime = first;
                firstTimeAppUsing = false;
                Preferences.Set("firstTimeAppUsing", firstTimeAppUsing.ToString());
            }

            if ((!times.Any(x => x.Hours == first.Hours && x.Minutes == first.Minutes)))
            {
                times.Add(first);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(times);
                Preferences.Set("times", json);
            }
            SetAndCancelAlarm();
        }
        private async void ChangeTimeButton_OnClick(object sender, EventArgs e) // Zmiana czasu alarmu
        {
            if (times.Count == 0)
            {
                await DisplayAlert("Ups!", "Najpierw dodaj godzinę do listy alarmów, naciskając +.", "OK");
                return;
            }
            else
            {
                times = times.OrderBy(x => x.TotalMinutes).ToList();
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
                        Text = buttonText,
                        BackgroundColor = Color.DarkBlue,
                        BorderColor = Color.White,
                        BorderWidth = 1,
                        HeightRequest = 80,
                        WidthRequest = 200,
                        FontSize = 35,
                        CornerRadius = 10,
                        FontAttributes = FontAttributes.Bold
                    };
                   
                    var binButton = new Button
                    {       
                        Text = "Usuń",
                        HeightRequest = 80,
                        WidthRequest = 100,
                        BorderColor = Color.White,
                        BorderWidth = 1,
                        CornerRadius = 10,
                        BackgroundColor = Color.DarkBlue,
                        FontSize = 20                       
                    };                  

                    var horizontalLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Children = { button, binButton }
                    };

                    stackLayout.Children.Add(horizontalLayout);
                    stackLayout.BackgroundColor = Color.Black;                                    
                    
                    binButton.Clicked += async (s, args) =>
                    {
                        bool answer = await DisplayAlert("Usuwanie", "Czy na pewno chcesz usunąć ten czas?", "Tak", "Nie");
                        if (answer)
                        {
                            TimeSpan timeSpan = TimeSpan.Parse(buttonText);
                            times = times.Where(x => x != timeSpan).ToList();
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(times);
                            Preferences.Set("times", json);
                            buttons.Remove(buttonText);
                            stackLayout.Children.Remove(horizontalLayout);
                            if (times.Count == 0)
                            {
                                await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
                            }
                        }
                    }; // Dodajemy obsługę zdarzenia kliknięcia w guzik kosza

                    button.Clicked += async (s, args) =>
                    {
                        TimeSpan timeSpan = TimeSpan.Parse(button.Text);
                        selectedTime = timeSpan;
                        if (selectedTime.ToString().Substring(0) == "0")
                            time.Text = selectedTime.ToString("h\\:mm");
                        else
                            time.Text = selectedTime.ToString("hh\\:mm");
                        Preferences.Set("TimeText", time.Text);
                        Preferences.Set("selectedTime", selectedTime.ToString());
                        await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
                    }; // Dodajemy obsługę zdarzenia kliknięcia w guzik z godziną
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
        }
        private void AddNewTimeButton_OnClick(object sender, EventArgs e)// Dodawanie nowego czasu alarmu
        {
            timePicker.Focus();
        }
        protected override void OnAppearing() // Wczytywanie danych z pamięci telefonu
        {
            INotificationService notificationService = DependencyService.Get<INotificationService>();
            base.OnAppearing();
            #region getPreferences
            isAlarmSet = bool.Parse(Preferences.Get("isAlarmSet", "false"));
            SetAndCancel.Text = Preferences.Get("setAndcancel", "Włącz Alarm");
            time.Text = Preferences.Get("TimeText", "00:00");
            selectedTime = TimeSpan.Parse(Preferences.Get("selectedTime", "00:00"));
            string json = Preferences.Get("times", "[]");
            times = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimeSpan>>(json);
            dayWhenAlarmGoes = DateTime.Parse(Preferences.Get("dayWhenAlarmGoes", "1.01.2023"));
            string json2 = Preferences.Get("recommendationList", "[]");
            recommendationList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<bool>>(json2);
            firstTimeAppUsing = bool.Parse(Preferences.Get("firstTimeAppUsing", "true"));            
            #endregion getPreferences

            if (isAlarmSet == true)
            {
                time.IsEnabled = false;
            }
            

            if (aboutPageSetPrefereces)
            {
                if (isAlarmSet && dayWhenAlarmGoes < DateTime.Now)
                {
                    Task.Run(async () => await Recommendation());
                }
                aboutPageSetPrefereces = false;
            }
            Timer();
        }
        private void Timer()
        {
            if (isAlarmSet)
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (DateTime.Now >= dayWhenAlarmGoes)
                    {
                        Task.Run(async () => await Recommendation());
                    }
                    return true;
                });
            }
        }
        public async Task Recommendation()
        {
            var x = await DisplayAlert("Alarm", "Czy wyspales sie?", "Tak", "Nie");
            if (x)
            {
                recommendationList.Add(true);
            }
            else
            {
                recommendationList.Add(false);
            }
            Preferences.Set("recommendationList", Newtonsoft.Json.JsonConvert.SerializeObject(recommendationList));
            dayWhenAlarmGoes = dayWhenAlarmGoes.AddDays(1);
            if (isAlarmSet)
            {
                IAlarmService alarmService = DependencyService.Get<IAlarmService>();
                var godzina = Algorithm(selectedTime.Hours, selectedTime.Minutes);
                alarmService.CancelAlarm();
                alarmService.CancelMedia();
                alarmService.SetAlarm(godzina.Item1, godzina.Item2);
            }
        }
        public (int, int) Algorithm(int hour, int minute)// Algorytm do ustawiania alarmu na najbliższą 1,5 minuty
        {
            DateTime teraz = DateTime.Now; // Pobierz aktualną godzinę i minutę

            DateTime podanaData = new DateTime(teraz.Year, teraz.Month, teraz.Day, hour, minute, 0); // Obiekt DateTime dla podanej daty            

            if (podanaData < teraz) // Jeżeli podana godzina jest mniejsza od teraźniejszej godziny
            {
                podanaData = podanaData.AddDays(1); // Dodaj 1 dzień do podanej godziny
            }

            teraz = teraz.AddMinutes(timeToFallAsleep); //dodaje czas zasniecia (25min)

            TimeSpan roznicaCzasu = podanaData - teraz; // Obliczam różnicę czasu między podaną godziną a teraźniejszą godziną

            double rem = 80;// Sredni czas fazy rem 

            double n = 0;

            for (int i = 0; i < recommendationList.Count; i++)
            {
                bool currentRecommendation = recommendationList[i];

                if (i == 0 && currentRecommendation)
                {
                    n = 5; // Jeśli pierwsza wartość jest true 
                }
                else if (currentRecommendation)
                {
                    break; // Jeśli wartość jest true, ale nie jest pierwsza
                }
                else
                {
                    n += 5; // Jeśli wartość jest false
                }
            }
            rem += n; //dodaje do fazy rem minuty wedlug tego co uzytkownik podawal w rekomendacjach
            int iloscIteracji = (int)(roznicaCzasu.TotalMinutes / rem); // Dziele różnicę czasu przez fazy rem

            DateTime najblizszyCzas = teraz; // Dodaje rem tyle razy, ile wynosi ilość iteracji
            for (int i = 0; i < iloscIteracji; i++)
            {
                najblizszyCzas = najblizszyCzas.AddMinutes(rem);
            }

            if (podanaData < teraz.AddMinutes(rem)) //jesli podana godzina jest nizsza niz jedna faza rem, budzik obudzi o podanej dacie.
            {
                return (podanaData.Hour, podanaData.Minute);
            }
            return (najblizszyCzas.Hour, najblizszyCzas.Minute);
        }

    }

}
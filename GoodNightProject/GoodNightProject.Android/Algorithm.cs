using System;
using Android.App;
using Android.Content;
using Android.Icu.Util;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Linq;
using GoodNightProject.Droid;
using Android.Provider;
using Android.Media;


[assembly: Xamarin.Forms.Dependency(typeof(GoodNightProject.Droid.Algorithm))]
namespace GoodNightProject.Droid
{
    public class Algorithm : IAlgorithm
    {
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
using GoodNightProject.ViewModels;
using GoodNightProject.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GoodNightProject
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();            
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

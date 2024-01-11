using GoodNightProject.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace GoodNightProject.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoodNightProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderTemplate : ContentView
    {
        public HeaderTemplate()
        {
            InitializeComponent();
        }
    }
}
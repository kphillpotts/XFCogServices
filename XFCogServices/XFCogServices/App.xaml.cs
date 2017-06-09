using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XFCogServices.ViewModels;

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace XFCogServices
{
    public partial class App : Application
    {

        private static MainViewModel mainViewModel = new MainViewModel();

        public static MainViewModel MainViewModel { get => mainViewModel; set => mainViewModel = value; }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new XFCogServices.MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

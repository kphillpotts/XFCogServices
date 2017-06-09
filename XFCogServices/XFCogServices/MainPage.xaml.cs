using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFCogServices
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = App.MainViewModel;
        }

        private async Task Button_ClickedAsync(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });


            App.MainViewModel.File = file;

            if (file == null)
                return;

            //await DisplayAlert("File Location", file.Path, "OK");

            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
//                file.Dispose();
                return stream;
            });

            //or:
            //image.Source = ImageSource.FromFile(file.Path);
            //image.Dispose();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AnalysisPage());
        }

        private void ImageSearch_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ImageSearch());
        }
    }
}

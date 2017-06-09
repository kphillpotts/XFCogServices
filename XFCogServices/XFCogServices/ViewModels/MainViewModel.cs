using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using MvvmHelpers;
using Newtonsoft.Json;
using Plugin.Media;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XFCogServices.Analyzer;
using XFCogServices.Models;

namespace XFCogServices.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public enum ImageSourceType
        {
            None = 0,
            Camera = 1,
            File = 2,
            Url = 3
        }

        private ImageSourceType imageSource = ImageSourceType.None;
        public ImageSourceType SelectedImageType
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        private bool analysisDone = false;
        public bool AnalysisDone
        {
            get { return analysisDone; }
            set { SetProperty(ref analysisDone, value); }
        }

        string fileName;
        public string FileName
        {
            get { return fileName; }
            set { SetProperty(ref fileName, value); }
        }



        private ObservableCollection<ClientItemModel> analysisResult = new ObservableCollection<ClientItemModel>();

        public ObservableCollection<ClientItemModel> AnalysisResult
        {
            get { return analysisResult; }
            set { SetProperty(ref analysisResult, value); }
        }


        Plugin.Media.Abstractions.MediaFile file;
        public Plugin.Media.Abstractions.MediaFile File
        {
            get { return file; }
            set {
                SetProperty(ref file, value);
                OnPropertyChanged("ImageSource");
            }
        }


        public ICommand CameraPhotoCommand { get; private set; }
        public ICommand DevicePhotoCommand { get; private set; }

        public ICommand AnalyzePhotoCommand { get; private set; }
        public ICommand ImageSearchCommand { get; private set; }


        public ImageSource ImageSource
        {
            get
            {
                if (SelectedImageType == ImageSourceType.Url)
                {
                    if (SelectedSearchImage != null)
                    {
                        return ImageSource.FromUri(new Uri(SelectedSearchImage.ImageLink));
                    }
                }

                if (File == null)
                {
                    return ImageSource.FromFile("ImagePlaceholder.png");
                }


                return ImageSource.FromStream(() =>
                {
                    var stream = File.GetStream();
                    //File.Dispose();
                    return stream;
                });
            }
        }

        private ImageResult _selectedSearchImage;
        public ImageResult SelectedSearchImage
        {
            get { return _selectedSearchImage; }
            set
            {
                SetProperty(ref _selectedSearchImage, value);
                if (value != null)
                {
                    SelectedImageType = ImageSourceType.Url;
                }
                OnPropertyChanged("ImageSource");
            }
        }

        public MainViewModel()
        {
            Images = new ObservableRangeCollection<ImageResult>();

            CameraPhotoCommand = new Command(async () => await GetPhotoFromCamera());
            DevicePhotoCommand = new Command(async () => await GetPhotoFromDevice());
            AnalyzePhotoCommand = new Command(async () => await AnalyzePhoto());
            ImageSearchCommand = new Command<string>(async s => await SearchForImagesAsync(s));
        }

        private async Task AnalyzePhoto()
        {
            try
            {
                IsBusy = true;
                var analysisHelper = new ClientHelper();
                List<ClientModel> clientModelList = new List<ClientModel>();

                List<ClientModel> analysisResult;
                if (SelectedSearchImage != null)
                {
                    analysisResult = await analysisHelper.GetImageDescriptionFromUrl(SelectedSearchImage.ImageLink, clientModelList);
                    var otherReuslt = await EmotionAnalysis.GetHappinessUrlAsync(SelectedSearchImage.ImageLink);
                }
                else
                {
                    analysisResult = await analysisHelper.GetImageDescription(File.GetStream(), clientModelList);

                }
                List<ClientItemModel> cim = new List<ClientItemModel>();

                AnalysisResult = new ObservableCollection<ClientItemModel>(analysisHelper.GetResultList(analysisResult, cim));

            }
            catch (Exception ex)
            {
                var page = new ErrorPopup(ex.Message);
                await PopupNavigation.PushAsync(page);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GetPhotoFromCamera()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                Directory = "Sample",
                Name = "test.jpg"
            });

            SelectedImageType = ImageSourceType.Camera;

            App.MainViewModel.File = file;

            if (file == null)
                return;
        }

        private async Task GetPhotoFromDevice()
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            var x = new ClientHelper();

            SelectedImageType = ImageSourceType.File;

            App.MainViewModel.File = file;

            if (file == null)
                return;



        }

        public ObservableRangeCollection<ImageResult> Images { get; }


        public async Task<bool> SearchForImagesAsync(string query)
        {
            //Bing Image API
            var url = $"https://api.cognitive.microsoft.com/bing/v5.0/images/" +
                      $"search?q={query}" +
                      $"&count=20&offset=0&mkt=en-us&safeSearch=Strict";

            try
            {
                var headerKey = "Ocp-Apim-Subscription-Key";
                var headerValue = XFCogServices.Services.ServiceKeys.BingSearch;

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(headerKey, headerValue);

                var json = await client.GetStringAsync(url);

                var stuff = JsonConvert.DeserializeObject<SearchResult>(json);

                var items = stuff.Images.Select(i => new ImageResult
                {
                    ContextLink = i.ContentUrl,
                    FileFormat = i.EncodingFormat,
                    ImageLink = i.ContentUrl,
                    ThumbnailLink = i.ThumbnailUrl ?? i.ContentUrl,
                    Title = i.Name
                });

                Images.ReplaceRange(items);

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

    }
}

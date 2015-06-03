using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Wp81CameraUniversal.ViewModel;

namespace Wp81CameraUniversal
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel DefaultViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            DefaultViewModel = new MainPageViewModel();
            DataContext = DefaultViewModel;

            DefaultViewModel.InitializeWebCam().ContinueWith((t) => SetDefaultCam());
        }

        public async void SetDefaultCam()
        {
            if (DefaultViewModel.WebcamList.Count != 0)
            {
                SetCaptureSource(DefaultViewModel.WebcamList[0].Id);
            }
            else
            {
                await ShowErrorMessage("No webcam available, please plug in a working one");
            }
        }

        /// <summary>
        /// Function running every time the user select a new item on the webcam list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newCamera = e.AddedItems[0] as DeviceInformation;
            SetCaptureSource(newCamera.Id);
        }

        public async void SetCaptureSource (String id)
        {
            try
            {
                var newCapture = await DefaultViewModel.InitializeMedia(id);

                // Set the source of the CaptureElement to your MediaCapture
                Capture.Source = newCapture;

                // Start the preview
                await newCapture.StartPreviewAsync();
            }
            catch (Exception ex)
            {
                // Will mainly happen because user didn't gave permission to access webcam
                ShowErrorMessage(ex.Message);
            }
        }

        public async Task ShowErrorMessage(string message)
        {
            await new MessageDialog(message).ShowAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Wp81CameraUniversal.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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

            StartWebCam();
        }

        public async Task ShowErrorMessage(string message)
        {
            await new MessageDialog(message).ShowAsync();
        }
   
        public async void StartWebCam()
        {
            try {
                // First need to find all webcams
                DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                if (webcamList.Count == 0)
                {
                    // if no webcam detected then we notify the user to add one
                    await ShowErrorMessage("No camera available. Please plug in a working camera.");
                }
                else
                {
                    // We look for the back webcam
                    DeviceInformation backWebcam = (from webcam in webcamList
                                                    where webcam.EnclosureLocation != null
                                                    && webcam.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back
                                                    select webcam).FirstOrDefault();

                    // Then you need to create a new MediaCapture
                    var newCapture = new MediaCapture();
                    // & initialize it
                    await newCapture.InitializeAsync(new MediaCaptureInitializationSettings
                    {
                        // Choose the webcam you want
                        VideoDeviceId = backWebcam.Id,
                        AudioDeviceId = "",
                        // We want to have the video
                        StreamingCaptureMode = StreamingCaptureMode.Video,
                        PhotoCaptureSource = PhotoCaptureSource.VideoPreview
                    });

                    // Set the source of the CaptureElement to your MediaCapture
                    Capture.Source = newCapture;

                    // Start the preview
                    await newCapture.StartPreviewAsync();
                }
            }
            catch (Exception ex)
            {
                // Most of the time an error here means the user refused
                // access to the webcam
                ShowErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Function running every time the user select a new item on the webcam list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newCamera = e.AddedItems[0] as DeviceInformation;

            var newCapture = new MediaCapture();
            await newCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                // Choose the webcam you want (backWebcam or frontWebcam)
                VideoDeviceId = newCamera.Id,
                AudioDeviceId = "",
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview
            });

            // Set the source of the CaptureElement to your MediaCapture
            Capture.Source = newCapture;

            // Start the preview
            await newCapture.StartPreviewAsync();

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
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

            this.NavigationCacheMode = NavigationCacheMode.Required;
            DefaultViewModel = new MainPageViewModel();
            DataContext = DefaultViewModel;

            StartWebCam();
        }

        public async void StartWebCam()
        {
            // First need to find all webcams
            DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Same for the back webcam
            DeviceInformation backWebcam = (from webcam in webcamList
                                            where webcam.EnclosureLocation != null
                                            && webcam.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back
                                            select webcam).FirstOrDefault();

            // Then you need to initialize your MediaCapture
            var newCapture = new MediaCapture();
            await newCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                // Choose the webcam you want (backWebcam or frontWebcam)
                VideoDeviceId = backWebcam.Id,
                AudioDeviceId = "",
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview
            });

            // Set the source of the CaptureElement to your MediaCapture
            Capture.Source = newCapture;

            // Start the preview
            await newCapture.StartPreviewAsync();
        }

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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

        }
    }
}

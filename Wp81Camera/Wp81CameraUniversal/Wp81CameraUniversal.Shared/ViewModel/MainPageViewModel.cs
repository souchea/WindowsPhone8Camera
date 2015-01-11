using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;

namespace Wp81CameraUniversal.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private MediaCapture _capture;
        public MediaCapture Capture
        {
            get { return _capture; }
            set
            {
                _capture = value;
                NotifyPropertyChanged("Capture");
            }
        }

        private DeviceInformationCollection _webcamList;
        public DeviceInformationCollection WebcamList
        {
            get { return _webcamList; }
            set
            {
                _webcamList = value;
                NotifyPropertyChanged("WebcamList");
            }
        }

        public MainPageViewModel()
        {
            InitializeWebCam();
        }

        public async void InitializeWebCam()
        {
            // First need to find all webcams
            WebcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Then I do a query to find the front webcam
            DeviceInformation frontWebcam = (from webcam in WebcamList
                                             where webcam.EnclosureLocation != null
                                             && webcam.EnclosureLocation.Panel == Panel.Front
                                             select webcam).FirstOrDefault();

            // Same for the back webcam
            DeviceInformation backWebcam = (from webcam in WebcamList
                                            where webcam.EnclosureLocation != null
                                            && webcam.EnclosureLocation.Panel == Panel.Back
                                            select webcam).FirstOrDefault();
        }
    }
}

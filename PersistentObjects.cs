using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace ChessPiecesDetection
{
    class PersistentObjects
    {
        private readonly string DEFAULT_WEBSERVICE_URL = "http://localhost:5000/predict";
        public Queue<PositionInstance> queuePositionInstances { get; set; }
        public WriteableBitmap originalLoadedImage { get; set; }
        public WriteableBitmap bitmapProcessingImage { get; set; }        
        public ImageCroppingView ImageCropperInstance { get; set; }
        public Boolean isCroppingImage { get; set; }
        public string predictionURLString { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PersistentObjects()
        {            
            reset();
        }

        /// <summary>
        /// Reset all persistent objects
        /// </summary>
        public void reset()
        {
            originalLoadedImage = null;
            bitmapProcessingImage = null;
            ImageCropperInstance = null;
            queuePositionInstances = null;
            isCroppingImage = false;
            predictionURLString = DEFAULT_WEBSERVICE_URL;

        }

    }
}

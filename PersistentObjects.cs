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
        public WriteableBitmap originalLoadedImage { get; set; }
        public WriteableBitmap bitmapProcessingImage { get; set; }        
        public ImageCropping ImageCropperInstance { get; set; }
        public Boolean isCroppingImage { get; set; }

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

        }

    }
}

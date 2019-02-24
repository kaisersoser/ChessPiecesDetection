using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using AForge;
using AForge.Imaging;
using AForge.Math.Geometry;
using AForge.Imaging.Filters;
using System.Collections.Generic;
using System.Drawing;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessPiecesDetection
{
    public sealed partial class PrepareImage : Page
    {
        PersistentObjects _LocalPersistentObject;

        /// <summary>
        /// Constructor
        /// </summary>
        public PrepareImage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loaded automatically when this window is loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _LocalPersistentObject = e.Parameter as PersistentObjects;

            if(_LocalPersistentObject!=null)
                if (_LocalPersistentObject.originalLoadedImage != null)
                {
                    MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject);
                }
        }

        /// <summary>
        /// Loads an image from the file system and calls the ImageLoaded XAML control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImageLoadButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                _LocalPersistentObject.bitmapProcessingImage = BitmapFactory.New((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                _LocalPersistentObject.bitmapProcessingImage.SetSource(stream);
                
                _LocalPersistentObject.originalLoadedImage = _LocalPersistentObject.bitmapProcessingImage;
            }

            MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject);
        }

        /// <summary>
        /// Use AForge to automatically detect the image borders, and crop it to size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoFitImage_Click(object sender, RoutedEventArgs e)
        {
            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

            WriteableBitmap bmp = _LocalPersistentObject.bitmapProcessingImage as WriteableBitmap;

            bmp = bmp.Resize(1024, 1024, WriteableBitmapExtensions.Interpolation.Bilinear);
            Bitmap image = AForge.Imaging.Image.Clone((Bitmap)bmp, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 50;
            blobCounter.MinWidth = 50;

            blobCounter.ProcessImage(image);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // check for rectangles
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints))
                {
                    // only do things if the corners form a rectangle
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Square)
                    {
                        // here i use the graphics class to draw an overlay, but you
                        // could also just use the cornerPoints list to calculate your
                        // x, y, width, height values.
                        List<AForge.Point> Points = new List<AForge.Point>();

                        bmp = bmp.Crop(cornerPoints[0].X,
                                       cornerPoints[0].Y,
                                       cornerPoints[2].X,
                                       cornerPoints[2].Y);           
                    }
                }
            }

            bmp = bmp.Resize(640, 640, WriteableBitmapExtensions.Interpolation.Bilinear);
            _LocalPersistentObject.bitmapProcessingImage = (WriteableBitmap)bmp;

            MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject, new SuppressNavigationTransitionInfo());
        }


        /// <summary>
        /// Reset the currently loaded image to its original instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///      
        private void ResetImage_Click(object sender, RoutedEventArgs e)
        {
            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

            _LocalPersistentObject.bitmapProcessingImage = _LocalPersistentObject.originalLoadedImage;
            MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject, new SuppressNavigationTransitionInfo());
        }


        /// <summary>
        /// Cropping tool for the Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private async void CropImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_LocalPersistentObject.isCroppingImage)
            {
                MainImageFrame.Navigate(typeof(ImageCropping), _LocalPersistentObject);
                CropImageButton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 245, 245, 220));    // Beige: 245, 245, 220
                CropImageButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));                
            }
            else
            {
                // MainImageFrame.Navigate(typeof(ImageCropping), _LocalPersistentObject);
                CropImageButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 245, 245, 220));    // Beige: 245, 245, 220
                CropImageButton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                _LocalPersistentObject.isCroppingImage = false;

                ImageCropping imageCroppingRef = _LocalPersistentObject.ImageCropperInstance as ImageCropping;

                _LocalPersistentObject.bitmapProcessingImage = await imageCroppingRef.GetCropppedImageBitmap();
                MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject, new SuppressNavigationTransitionInfo());
            }
        }
    }
}

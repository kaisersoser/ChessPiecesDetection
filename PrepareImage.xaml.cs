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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

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
            _LocalPersistentObject = new PersistentObjects();
            _LocalPersistentObject.BoardPositions = new ObservableCollection<BoardPosition>();
            _LocalPersistentObject.isCroppingImage = false;
        }

        /// <summary>
        /// Loaded automatically when this window is loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _LocalPersistentObject = e.Parameter as PersistentObjects;

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

            Bitmap image = AForge.Imaging.Image.Clone((Bitmap)_LocalPersistentObject.bitmapProcessingImage, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            /*
            DifferenceEdgeDetector deFilter = new DifferenceEdgeDetector();
            deFilter.ApplyInPlace(image);
            */

            /*
            Dilatation diFilter = new Dilatation();
            diFilter.ApplyInPlace(image);            
            */

            
            Threshold biFilter = new Threshold(40);
            biFilter.ApplyInPlace(image);
            

            /*
            ConnectedComponentsLabeling ccFilter = new ConnectedComponentsLabeling();
            image = ccFilter.Apply(image);
            */

            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = false;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;

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

                        _LocalPersistentObject.bitmapProcessingImage.DrawRectangle(cornerPoints[0].X,
                                          cornerPoints[0].Y,
                                          cornerPoints[2].X,
                                          cornerPoints[2].Y,
                                          Windows.UI.Colors.Red);
                        
                        foreach (var point in cornerPoints)
                        {
                            Points.Add(new AForge.Point(point.X, point.Y));
                        }
                        
                        //GraphicsUnit g = Graphics.FromImage(image);
                        //g.DrawPolygon(new Pen(Color.Red, 5.0f), Points.ToArray());

                        //image.Save("result.png");                   
                    }
                }
            }

            _LocalPersistentObject.bitmapProcessingImage = (WriteableBitmap)image;
            MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject);
        }


        /// <summary>
        /// Reset the currently loaded image to its original instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        /*
        private void ResetImage_Click(object sender, RoutedEventArgs e)
        {
            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

            ImageView.Source = _LocalPersistentObject.originalLoadedImage;
        } */



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
                MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject);
            }
        }
    }
}

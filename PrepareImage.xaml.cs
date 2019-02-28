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
using Windows.UI;

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
            {
                if (_LocalPersistentObject.originalLoadedImage != null)
                {
                    MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject);
                }
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
        /// Applies a HoughTransform detection on the image to determine image edges and corners
        /// Not currently used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HoughTransformProcessBoard_Click(object sender, RoutedEventArgs e)
        {
            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

            WriteableBitmap bmp = _LocalPersistentObject.bitmapProcessingImage;

            Bitmap image = AForge.Imaging.Image.Clone((Bitmap)bmp, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);


            HoughLineTransformation lineTransform = new HoughLineTransformation();
            // apply Hough line transofrm
            lineTransform.ProcessImage(image);
            Bitmap houghLineImage = lineTransform.ToBitmap();
            // get lines using relative intensity
            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(0.487);

            int numlines = 0;
            foreach (HoughLine line in lines)
            {
                // get line's radius and theta values
                int r = line.Radius;
                double t = line.Theta;

                // check if line is in lower part of the image
                if (r < 0)
                {
                    t += 180;
                    r = -r;
                }

                // convert degrees to radians
                t = (t / 180) * Math.PI;

                // get image centers (all coordinate are measured relative
                // to center)
                int w2 = image.Width / 2;
                int h2 = image.Height / 2;

                double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                if (line.Theta != 0)
                {
                    // none-vertical line
                    x0 = -w2; // most left point
                    x1 = w2;  // most right point

                    // calculate corresponding y values
                    y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                    y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                }
                else
                {
                    // vertical line
                    x0 = line.Radius;
                    x1 = line.Radius;

                    y0 = h2;
                    y1 = -h2;
                }
                int X0 = (int)x0 + w2;
                int Y0 = (int)h2 - (int)y0;
                int X1 = (int)x1 + w2;
                int Y1 = h2 - (int)y1;

                bmp.DrawRectangle(X0,
                                  Y0,
                                  X1,
                                  Y1,
                                  Colors.Red);

                numlines++;
            }

            // Now perform blob detection
            
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

                        bmp.DrawRectangle(cornerPoints[0].X,
                                          cornerPoints[0].Y,
                                          cornerPoints[2].X,
                                          cornerPoints[2].Y,
                                          Colors.Green);

                        foreach (var point in cornerPoints)
                        {
                            Points.Add(new AForge.Point(point.X, point.Y));
                        }
                    }
                }
            }

            _LocalPersistentObject.bitmapProcessingImage = (WriteableBitmap)bmp;
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
            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

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

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using DataAccessLibrary;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessPiecesDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PersistentObjects _PersistentObjects;
        public MainPage()
        {
            this.InitializeComponent();
            _PersistentObjects = new PersistentObjects();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }


        


        /*
        private void ProcessBoard1_Click(object sender, RoutedEventArgs e)
        {
            if (bmp == null)
                return;

            Bitmap image = AForge.Imaging.Image.Clone((Bitmap)bmp, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            DifferenceEdgeDetector deFilter = new DifferenceEdgeDetector();
            deFilter.ApplyInPlace(image);

            Dilatation diFilter = new Dilatation();
            diFilter.ApplyInPlace(image);

            Threshold biFilter = new Threshold(10);
            biFilter.ApplyInPlace(image);

            ConnectedComponentsLabeling ccFilter = new ConnectedComponentsLabeling();
            image = ccFilter.Apply(image);

            /*
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
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

                        bmp.DrawRectangle(cornerPoints[0].X,
                                          cornerPoints[0].Y,
                                          cornerPoints[2].X,
                                          cornerPoints[2].Y,
                                          Colors.Red);

                        

                        
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
            
            ImageView.Source = (WriteableBitmap)image;
        } */

        /*
        private void HoughTransformProcessBoard_Click(object sender, RoutedEventArgs e)
        {
            if (bmp == null)
                return;

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
            /*
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
            
            ImageView.Source = (WriteableBitmap)bmp;
        }*/
        /*
        private void DetectBoard_Click(object sender, RoutedEventArgs e)
        {
            if (bmp == null)
                return;

            bmp = bmp.Resize(1024, 1024, WriteableBitmapExtensions.Interpolation.Bilinear);

            Bitmap image = AForge.Imaging.Image.Clone((Bitmap)bmp, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

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

                        bmp = bmp.Crop(cornerPoints[0].X,
                                            cornerPoints[0].Y,
                                            cornerPoints[2].X,
                                            cornerPoints[2].Y
                                            );
                    }
                }
            }

            bmp = bmp.Resize(512, 512, WriteableBitmapExtensions.Interpolation.Bilinear);
            ProcessBoard(bmp);

            ImageView.Source = (WriteableBitmap)bmp;
        }*/

        /*
        private async void ProcessBoard(WriteableBitmap _bmp)
        {
            String[] verticalPos = { "a", "b", "c", "d", "e", "f", "g", "h" };

            WriteableBitmap originalBitmap = _bmp;
            WriteableBitmap modifiedBitmap = null;
            BoardPosition bPos = null;
            int posSize = 64;

            BoardPositions.Clear();

            int x0 = 0, y0 = 0;

            for (int v = 7; v >= 0; v--)
            {
                for (int h = 1; h <= 8; h++)
                {
                    x0 = (h - 1) * posSize;
                    y0 = (8 - (v + 1)) * posSize;
                    //x1 = h * posSize;
                    //y1 = (8-v) * posSize;

                    bPos = new BoardPosition();
                    bPos.PositionID = verticalPos[(h - 1)] + (v + 1);
                    bPos.PositionImage = new WriteableBitmap(posSize, posSize);
                    modifiedBitmap = originalBitmap.Crop(x0, y0, posSize, posSize);
                    bPos.PositionImage = modifiedBitmap;
                    bPos.PieceID = (int)BoardPosition.Pieces.EPY;

                    modifiedBitmap = modifiedBitmap.Resize(64, 64, WriteableBitmapExtensions.Interpolation.Bilinear);
                    modifiedBitmap = modifiedBitmap.Gray();
                    bPos.PositionImageByte = await EncodeJpeg(modifiedBitmap);
                    bPos.PositionImage = modifiedBitmap;
                    bPos.PieceName = "Empty";
                    BoardPositions.Add(bPos);


                }
            }
        } */

        /*
        private void PieceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                int pieceIDPos = 0;
                ComboBox currentComboBox = (ComboBox)sender;

                var item = (ComboBoxItem)currentComboBox.SelectedItem;
                BoardPosition bPos = (BoardPosition)item.DataContext;
                String strPieceName = item.Content.ToString();

                if (bPos != null)
                {
                    pieceIDPos = Array.IndexOf(bPos.PiecesNames, strPieceName);
                    bPos.PieceID = (int)Enum.GetValues(typeof(BoardPosition.Pieces)).GetValue(pieceIDPos);
                    bPos.PieceName = strPieceName;
                }

            }

        } */

        /*
        private async Task<byte[]> EncodeJpeg(WriteableBitmap bmp)
        {
            SoftwareBitmap soft = SoftwareBitmap.CreateCopyFromBuffer(bmp.PixelBuffer, BitmapPixelFormat.Bgra8, bmp.PixelWidth, bmp.PixelHeight);
            byte[] array = null;

            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, ms);
                encoder.SetSoftwareBitmap(soft);

                try
                {
                    await encoder.FlushAsync();
                }
                catch { }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }

            return array;
        }*/

        /*
        private void SaveBoard_Click(object sender, RoutedEventArgs e)
        {
            // Return if no image loaded
            if (bmp == null)
                return;

            // Return if no image processed
            if (BoardPositions.Count <= 0)
                return;

            List<BoardPosition> _boardPositions = BoardPositions.ToList<BoardPosition>();

            foreach (BoardPosition bp in _boardPositions)
            {
                DataAccess.AddData(bp.PositionID, bp.PositionImageByte, bp.PieceID, bp.PieceName);
            }

        }*/


        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadFile.IsSelected)
            {
                VisibleFrame.Navigate(typeof(PrepareImage), _PersistentObjects);
            }
            else if (ProcessItem.IsSelected)
            {
                VisibleFrame.Navigate(typeof(ProcessFile), _PersistentObjects);
            }
        }
    }
}

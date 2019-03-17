using System;
using Microsoft.Graphics.Canvas;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ChessPiecesDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditImageView : Page
    {
        PersistentObjects _LocalPersistentObject;
        ImageInstance item;
        CultureInfo culture = CultureInfo.CurrentCulture;
        WriteableBitmap ImageSource;

        public EditImageView()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _LocalPersistentObject = e.Parameter as PersistentObjects;

            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

            item = new ImageInstance(_LocalPersistentObject.bitmapProcessingImage);
            ImageSource = await item.GetImageSourceAsync();            
            await LoadBrushAsync();

            ThumbnailImage.Source = item.ImageBitmap;
            base.OnNavigatedTo(e);
        }

        private async Task LoadBrushAsync()
        {
            using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                await ImageSource.ToStream(stream, BitmapEncoder.BmpEncoderId);
                stream.Seek(0);
                ImageEffectsBrush.LoadImageFromStream(stream);
            }
        }

        private async void ExportImage()
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (CanvasRenderTarget offscreen = new CanvasRenderTarget(device, item.ImageBitmap.PixelWidth, item.ImageBitmap.PixelHeight, 96))
            {
                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await item.ImageBitmap.ToStream(stream, BitmapEncoder.BmpEncoderId);                    
                    using (CanvasBitmap image = await CanvasBitmap.LoadAsync(offscreen, stream))
                    {
                        ImageEffectsBrush.SetSource(image);

                        using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                        {
                            ds.Clear(Windows.UI.Colors.Black);

                            var img = ImageEffectsBrush.Image;
                            ds.DrawImage(img);
                        }
                    }

                    //stream.Dispose();
                }

                using (IRandomAccessStream outstream = new InMemoryRandomAccessStream())
                {
                    WriteableBitmap writeableBitmap = new WriteableBitmap(ImageSource.PixelWidth, ImageSource.PixelHeight);                    

                    await offscreen.SaveAsync(outstream, CanvasBitmapFileFormat.Bmp);
                    outstream.Seek(0);

                    writeableBitmap.SetSource(outstream);

                    _LocalPersistentObject.bitmapProcessingImage = writeableBitmap;
                    ThumbnailImage.Source = _LocalPersistentObject.bitmapProcessingImage;
                }               
            }
                 
                 
        }
    }
}

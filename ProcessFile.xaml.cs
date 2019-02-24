using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessPiecesDetection
{

    public sealed partial class ProcessFile : Page
    {
        PersistentObjects _LocalPersistentObject;
        public ObservableCollection<PositionInstance> BoardPositions { get; set; }

        public ProcessFile()
        {
            this.InitializeComponent();
            BoardPositions = new System.Collections.ObjectModel.ObservableCollection<PositionInstance>();
        }

        /// <summary>
        /// Loaded automatically when this window is loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _LocalPersistentObject = e.Parameter as PersistentObjects;

            /*
            if (_LocalPersistentObject != null)
                if (_LocalPersistentObject.originalLoadedImage != null)
                {
                    MainImageFrame.Navigate(typeof(ImageLoaded), _LocalPersistentObject);
                }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_bmp"></param>
        private async void ProcessBoard(WriteableBitmap _bmp)
        {
            String[] verticalPos = { "a", "b", "c", "d", "e", "f", "g", "h" };

            WriteableBitmap originalBitmap = _bmp;
            WriteableBitmap modifiedBitmap = null;
            PositionInstance bPos = null;
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

                    bPos = new PositionInstance();
                    bPos.PositionID = verticalPos[(h - 1)] + (v + 1);
                    bPos.PositionImage = new WriteableBitmap(posSize, posSize);
                    modifiedBitmap = originalBitmap.Crop(x0, y0, posSize, posSize);
                    bPos.PositionImage = modifiedBitmap;
                    bPos.PieceID = (int)PositionInstance.Pieces.EPY;

                    modifiedBitmap = modifiedBitmap.Resize(64, 64, WriteableBitmapExtensions.Interpolation.Bilinear);
                    modifiedBitmap = modifiedBitmap.Gray();
                    bPos.PositionImageByte = await EncodeJpeg(modifiedBitmap);
                    bPos.PositionImage = modifiedBitmap;
                    bPos.PieceName = "Empty";
                    BoardPositions.Add(bPos);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
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
        }

        private void PieceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}

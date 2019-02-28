using DataAccessLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
        StringBuilder _ConsoleStringBuffer;
        private int _ConsoleLineNumber;
        public ObservableCollection<PositionInstance> BoardPositions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessFile()
        {
            this.InitializeComponent();
            BoardPositions = new ObservableCollection<PositionInstance>();
            _ConsoleStringBuffer = new StringBuilder();
            UpdateConsole("Starting Image Processing...");
        }

        /// <summary>
        /// Loaded automatically when this window is loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _LocalPersistentObject = e.Parameter as PersistentObjects;

            
            if (_LocalPersistentObject != null)
                if (_LocalPersistentObject.bitmapProcessingImage != null)
                {
                    ProcessBoard(_LocalPersistentObject.bitmapProcessingImage);
                }
            
        }



    
        /// <summary>
        /// Processes the Board image into individual pieces that can be selected and identified seprately.
        /// </summary>
        /// <param name="_bmp"></param>
        private async void ProcessBoard(WriteableBitmap _bmp)
        {
            String[] verticalPos = { "a", "b", "c", "d", "e", "f", "g", "h" };

            WriteableBitmap originalBitmap = _bmp;
            WriteableBitmap modifiedBitmap = null;
            PositionInstance bPos = null;
            int posSize = 64;

            UpdateConsole("Processing Board Image into Pieces...");

            originalBitmap = originalBitmap.Resize(512, 512, WriteableBitmapExtensions.Interpolation.Bilinear);

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
            UpdateConsole("Processing Done...");
        }

        /// <summary>
        /// Encodes a Bitmap object into a byte array.
        /// Useful for sending images via JSON to a restful service
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



        /// <summary>
        /// Add log information for the console string buffer
        /// </summary>
        /// <param name="info"></param>
        private void UpdateConsole(String info)
        {
            if (_ConsoleStringBuffer == null)
                return;
            _ConsoleLineNumber++;

            _ConsoleStringBuffer.AppendFormat("{0}: {1}", _ConsoleLineNumber,info);
            _ConsoleStringBuffer.Append(Environment.NewLine);
            ConsoleInfo.Text = _ConsoleStringBuffer.ToString();
        }

        /// <summary>
        /// Update the Piece label identity everytime it is changed manually by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                int pieceIDPos = 0;
                ComboBox currentComboBox = (ComboBox)sender;

                var item = (ComboBoxItem)currentComboBox.SelectedItem;
                PositionInstance bPos = (PositionInstance)item.DataContext;
                String strPieceName = item.Content.ToString();

                if (bPos != null)
                {
                    pieceIDPos = Array.IndexOf(bPos.PiecesNames, strPieceName);
                    bPos.PieceID = (int)Enum.GetValues(typeof(PositionInstance.Pieces)).GetValue(pieceIDPos);
                    bPos.PieceName = strPieceName;
                }

            }

        }

        /// <summary>
        /// Write the configured table into a database table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteToDBButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            String pieceDescriptionStr;

            // Return if no image loaded
            if (_LocalPersistentObject.bitmapProcessingImage == null)
                return;

            // Return if no image processed
            if (BoardPositions.Count <= 0)
                return;

            
            UpdateConsole("Start writing each piece as a row in the database");

            // Converts the Observable Collections into a List Object
            List<PositionInstance> _boardPositions = BoardPositions.ToList<PositionInstance>();

            // Write each position into the database
            foreach (PositionInstance bp in _boardPositions)
            {
                pieceDescriptionStr = new StringBuilder().AppendFormat("Adding {0} label at {1}", bp.PieceName, bp.PositionID).ToString();
                DataAccess.AddData(bp.PositionID, bp.PositionImageByte, bp.PieceID, bp.PieceName);
                UpdateConsole(pieceDescriptionStr);
            }
            
            int count = DataAccess.GetNumberOfItemsInDB();            
            
            UpdateConsole(new StringBuilder().AppendFormat("There are {0} total rows in the database",count).ToString());
            UpdateConsole("Database Update Complete");
        }
    }
}

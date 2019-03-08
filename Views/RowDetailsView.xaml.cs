using DataAccessLibrary;
using System.IO;
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
    public sealed partial class RowDetailsView : Page
    {
        private PiecesTableRowInstance RowInstanceDetails;

        public RowDetailsView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loaded automatically when this window is loaded
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RowInstanceDetails = e.Parameter as PiecesTableRowInstance;

            if (RowInstanceDetails == null)
                return;

            Row_ID_Details.Text = RowInstanceDetails.RowID.ToString();
            Position_ID_Details.Text = RowInstanceDetails.PositionID;
            Piece_ID_Details.Text = RowInstanceDetails.PieceID.ToString();
            Piece_Name_Details.Text = RowInstanceDetails.PieceName;

            byte[] reader = (byte[]) RowInstanceDetails.PositionImageByte;
            MemoryStream buf = new MemoryStream (reader);
            buf.Position = 0;

            WriteableBitmap image = await BitmapFactory.FromStream(buf);

            PieceImage.Source = image;
        }
    }
}

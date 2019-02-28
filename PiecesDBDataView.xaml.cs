using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls;
using DataAccessLibrary;
using Windows.UI.Xaml.Navigation;
using System.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ChessPiecesDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PiecesDBDataView : Page
    {
        int numberOfRows = 0;        

        public PiecesDBDataView()
        {
            this.InitializeComponent();
            numberOfRows = DataAccess.GetNumberOfItemsInDB();
            RowsInDB.Text = numberOfRows + " rows in DB";            
        }


        /// <summary>
        /// Loads rows from the database
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RowsInDB.Text = "Loading data from PiecesTable...";
            PiecesTableDataGrid.ItemsSource = DataAccess.GetData().DefaultView;

            RowsInDB.Text = DataAccess.GetNumberOfItemsInDB() + " rows in DB";
        }
    }
}

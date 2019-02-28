using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.UI.Xaml.Controls;
using DataAccessLibrary;
using Windows.UI.Xaml.Navigation;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ChessPiecesDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PiecesDBDataView : Page
    {
        /// <summary>
        /// Number of rows in the PiecesTable
        /// </summary>
        int numberOfRows = 0;

        /// <summary>
        /// Current start row displayed
        /// </summary>
        int currentRowStartPos = 0;

        /// <summary>
        /// Current end row displayed
        /// </summary>
        int currentRowEndPos = 0;

        /// <summary>
        /// Blocks of rows displayed in the table
        /// </summary>
        private readonly int blockOfRows = 100;

        /// <summary>
        /// ObservableCollection of rows representing pieces in the database table
        /// </summary>
        public ObservableCollection<PiecesTableRowInstance> PiecesTableDataSource { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PiecesDBDataView()
        {
            this.InitializeComponent();
            numberOfRows = DataAccess.GetNumberOfItemsInDB();
            RowsInDB.Text = numberOfRows + " rows in DB";
            PiecesTableDataSource = new ObservableCollection<PiecesTableRowInstance>();
        }


        /// <summary>
        /// Loads rows from the database
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RowsInDB.Text = "Loading data from PiecesTable...";
            UpdateRowsFromPiecesTableDB(_startRow:0, _endRow:blockOfRows);

            RowsInDB.Text = DataAccess.GetNumberOfItemsInDB() + " rows in DB";
        }

        /// <summary>
        /// Nove to the first block of dataset in the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToFirst_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            currentRowStartPos = 0;
            currentRowEndPos = blockOfRows;

            // Update the ObservableCollections
            UpdateRowsFromPiecesTableDB(_startRow: currentRowStartPos, _endRow: currentRowEndPos);
        }

        /// <summary>
        /// Move to the previous block of data in the dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveBackward_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            currentRowStartPos = currentRowStartPos - blockOfRows;
            currentRowEndPos = currentRowEndPos - blockOfRows;

            if (currentRowStartPos < 0)
                currentRowStartPos = 0;
            if (currentRowEndPos < 0)
                currentRowEndPos = blockOfRows;

            // Update the ObservableCollections
            UpdateRowsFromPiecesTableDB(_startRow: currentRowStartPos, _endRow: currentRowEndPos);
        }

        /// <summary>
        /// Move forward by a certain number of blocks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveForeward_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            currentRowStartPos = currentRowStartPos + blockOfRows;
            currentRowEndPos = currentRowEndPos + (blockOfRows * 2);

            if (currentRowStartPos >= numberOfRows)
                currentRowStartPos = 0;
            if (currentRowEndPos >= numberOfRows)
                currentRowEndPos = numberOfRows;

            // Update the ObservableCollections
            UpdateRowsFromPiecesTableDB(_startRow: currentRowStartPos, _endRow: currentRowEndPos);
        }

        /// <summary>
        /// Move to the last block of data in the dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToLast_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            currentRowStartPos = numberOfRows-blockOfRows;
            currentRowEndPos = numberOfRows;

            // If the dataset is less than the block size, then set the currentRowStartPos = 0
            if (currentRowStartPos < 0)
                currentRowStartPos = 0;

            // Update the ObservableCollections
            UpdateRowsFromPiecesTableDB(_startRow: currentRowStartPos, _endRow: currentRowEndPos);
        }


        /// <summary>
        /// Queries the database from Start to end Row, returning the rows in the database
        /// </summary>
        /// <param name="_startRow"></param>
        /// <param name="_endRow"></param>
        private void UpdateRowsFromPiecesTableDB(int _startRow = 0, int _endRow = 0)
        {
            // Clears the observable collection
            PiecesTableDataSource.Clear();
            List<PiecesTableRowInstance> results = DataAccess.GetData(startRow: _startRow, endRow: _endRow);

            foreach (var row in results)
            {
                PiecesTableDataSource.Add(row);
            }            
        }

    }
}

using System;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ChessPiecesDetection.Controls
{
    public sealed partial class SelectPiece : UserControl
    {
        public SelectPiece()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Update the selected piece object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PositionInstance position = (PositionInstance)((ListBox)sender).DataContext;

            var selectedObject = ((ListBoxItem)((ListBox)sender).SelectedItem).Content;

            // Is Empty Piece
            if (((ListBoxItem)((ListBox)sender).SelectedItem).Name.Equals("Empty", StringComparison.CurrentCultureIgnoreCase))
                position.PieceID = (int)PositionInstance.PieceEnum.EPY;
            // Is Black Pawn Selected
            if (selectedObject is BPawnPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.BPA;
            // Is Black Bishop Selected
            else if(selectedObject is BBishopPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.BBS;
            // Is Black Knight Selected
            else if (selectedObject is BKnightPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.BKN;
            // Is Black Rook Selected
            else if (selectedObject is BRookPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.BRK;
            // Is Black Queen Selected
            else if (selectedObject is BQueenPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.BQN;
            // Is Black King Selected
            else if (selectedObject is BKingPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.BKG;
            // Is White Pawn Selected
            else if (selectedObject is WPawnPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.WPA;
            // Is White Bishop Selected
            else if (selectedObject is WBishopPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.WBS;
            // Is White Knight Selected
            else if (selectedObject is WKnightPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.WKN;
            // Is White Rook Selected
            else if (selectedObject is WRookPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.WRK;
            // Is White Queen Selected
            else if (selectedObject is WQueenPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.WQN;
            // Is White King Selected
            else if (selectedObject is WKingPiece)
                position.PieceID = (int)PositionInstance.PieceEnum.WKG;

            // Update the Position KeyID
            position.PieceKeyID = PositionInstance.GetPieceKeyFromPieceID(position.PieceID);
            position.PieceName = PositionInstance.GetPieceNameFromPieceID(position.PieceID);
        }
    }
}

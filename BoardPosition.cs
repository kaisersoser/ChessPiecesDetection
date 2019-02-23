using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ChessPiecesDetection
{
    public class BoardPosition
    {
        public enum Pieces
        {
            EPY = 0,      // Empty Square
            WPA = 110,      // White Pawn
            WKN = 111,      // White Knight
            WBS = 112,      // White Bishop
            WRK = 113,      // White Rook
            WQN = 114,      // White Queen
            WKG = 115,      // White King
            BPA = 210,      // Black Pawn
            BKN = 211,      // Black Knight
            BBS = 212,      // Black Bishop
            BRK = 213,      // Black Rook
            BQN = 214,      // Black Queen
            BKG = 215,      // Black King
        }

        private readonly String[] piecesNames = { "Empty",
                                        "White Pawn",
                                        "White Knight",
                                        "White Bishop",
                                        "White Rook",
                                        "White Queen",
                                        "White King",
                                        "Black Pawn",
                                        "Black Knight",
                                        "Black Bishop",
                                        "Black Rook",
                                        "Black Queen",
                                        "Black King"};


        public string PositionID { get; set; }
        public WriteableBitmap PositionImage { get; set; }
        public byte[] PositionImageByte { get; set; }
        public int PieceID { get; set; }

        public string[] PiecesNames => piecesNames;

    }
}

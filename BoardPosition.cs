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
            EPY = 0,      // Empty Square 0
            WPA = 1,      // White Pawn 110
            WKN = 2,      // White Knight 111
            WBS = 3,      // White Bishop 112
            WRK = 4,      // White Rook 113
            WQN = 5,      // White Queen 114
            WKG = 6,      // White King 115
            BPA = 7,      // Black Pawn 210
            BKN = 8,      // Black Knight 211
            BBS = 9,      // Black Bishop 212
            BRK = 10,      // Black Rook 213
            BQN = 11,      // Black Queen 214
            BKG = 12,      // Black King 215
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
        public string PieceName { get; set; }

        public string[] PiecesNames => piecesNames;

    }
}

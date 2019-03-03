using System;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace ChessPiecesDetection
{
    /// <summary>
    /// Instance of a piece on the board.
    /// Contains information such as the piece type, name, position, image and predicted piece type.
    /// </summary>
    [DataContract]
    public class PositionInstance
    {
        public PositionInstance()
        {
            Reinitialize();
        }

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

        private static readonly String[] piecesNames = {
                                        "Empty",
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

        /// <summary>
        /// Position of the piece on the board
        /// </summary>
        [DataMember]
        public string PositionID { get; set; }
        /// <summary>
        /// Instance of image of piece as a writeable bitmap
        /// </summary>
        public WriteableBitmap PositionImage { get; set; }
        /// <summary>
        /// Instance of image as a byte array
        /// </summary>
        [DataMember]
        public byte[] PositionImageByte { get; set; }
        /// <summary>
        /// Piece Identification (See Enum:Pieces for types)
        /// </summary>
        [DataMember]
        public int PieceID { get; set; }
        /// <summary>
        /// Name/Label of Piece (See PiecesNames for labels)
        /// </summary>
        [DataMember]
        public string PieceName { get; set; }
        /// <summary>
        /// PredictedPieceID is the predicted type of piece returned by the web service
        /// </summary>
        [DataMember]
        public int PredictedPieceID { get; set; }

        /// <summary>
        /// Flag indicating if a piece has been predicted or not
        /// </summary>
        public bool IsPredicted { get; set; }

        /// <summary>
        /// Flag indicating if a piece has been badly predicted
        /// Used for statistics
        /// </summary>
        public bool IsWrongPrediction { get; set; }
        public static string[] PiecesNames => piecesNames;

        /// <summary>
        /// ReInitializes the PositionInstance Object
        /// </summary>
        public void Reinitialize()
        {
            PositionID = null;
            PositionImage = null;
            PositionImageByte = null;
            PieceID = 0;
            PieceName = null;
            PredictedPieceID = 0;
            IsPredicted = false;
            IsWrongPrediction = false;
        }
    }
}

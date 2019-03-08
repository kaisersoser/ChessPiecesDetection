using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChessPiecesDetection
{
    [DataContract]
    class PredictedPiece
    {
        /// <summary>
        /// Position of the piece on the board
        /// </summary>
        [DataMember]
        public string PositionID { get; set; }
        /// <summary>
        /// PredictedPieceID is the predicted type of piece returned by the web service
        /// </summary>
        [DataMember]
        public int PredictedPieceID { get; set; }
    }
}

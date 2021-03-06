﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary
{
    /// <summary>
    /// Represents an instance of a row in the PiecesTable
    /// </summary>
    public class PiecesTableRowInstance
    {
        public int RowID { get; set; }
        public string PositionID { get; set; }
        public byte[] PositionImageByte { get; set; }
        public int PieceID { get; set; }
        public string PieceName { get; set; }

    }
}

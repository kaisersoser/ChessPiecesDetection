using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace DataAccessLibrary
{
    public static class DataAccess
    {
        const String _STR_TABLE_NAME = "PiecesTable";
        const String _STR_DB_FILENAME = "PiecesSQLite.db";
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=" + _STR_DB_FILENAME))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                                      "EXISTS "+ _STR_TABLE_NAME + " (Primary_Key INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                      "PositionID VARCHAR(3) NULL, "+
                                      "ImageData BLOB NULL, "+
                                      "PieceID int, "+
                                      "PieceName VARCHAR(255) NULL"+
                                      ")";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public static void AddData(string _positionID, byte[] _imageData, int _pieceID, string _pieceName)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=" + _STR_DB_FILENAME))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO "+_STR_TABLE_NAME+" VALUES (NULL, @_PositionID, @_ImageData, @_PieceID, @_PieceName);";
                insertCommand.Parameters.AddWithValue("@_PositionID", _positionID);
                insertCommand.Parameters.AddWithValue("@_ImageData", _imageData);
                insertCommand.Parameters.AddWithValue("@_PieceID", _pieceID);
                insertCommand.Parameters.AddWithValue("@_PieceName", _pieceName);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }
    }
}

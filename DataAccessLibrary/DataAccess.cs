using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;

namespace DataAccessLibrary
{
    public static class DataAccess
    {
        const String _STR_TABLE_NAME = "PiecesTable";
        const String _STR_DB_FILENAME = "PiecesSQLite.db";

        /// <summary>
        /// InitializeDatabase is called at the start of the application.
        /// Onlce create a new database table if no database instance was found.
        /// </summary>
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

        /// <summary>
        /// Returns a count of the number of items in the database.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfItemsInDB()
        {
            // Initialize the count variable to be returned
            int count = 0;            

            // Start connection to db
            using (SqliteConnection db =
                new SqliteConnection("Filename=" + _STR_DB_FILENAME))
            {
                // open connection
                db.Open();

                SqliteCommand queryCommand = new SqliteCommand();
                queryCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                queryCommand.CommandText = "SELECT COUNT(*) FROM " + _STR_TABLE_NAME;
                // Execute command and convert response to Int
                count = Convert.ToInt32(queryCommand.ExecuteScalar());
                // Close connection
                db.Close();
            }
            // return result(count)
            return count;
        }

        /// <summary>
        /// Adds a row to the database
        /// </summary>
        /// <param name="_positionID"></param>
        /// <param name="_imageData"></param>
        /// <param name="_pieceID"></param>
        /// <param name="_pieceName"></param>
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

        /// <summary>
        /// Returns a List<PiecesTableRowInstance> representing all rows in the table PiecesTable
        /// </summary>
        /// <returns></returns>
        public static List<PiecesTableRowInstance> GetData(int startRow=0, int endRow=0) 
        {
            if (startRow < 0 || endRow < 0)
                throw new Exception("Query range must start and end with a row number greater than 0");

            // Instantiate a new instance of the results List
            List<PiecesTableRowInstance> results = new List<PiecesTableRowInstance>();
            PiecesTableRowInstance row = null;

            // Start SQL Connection
            using (SqliteConnection db =
                new SqliteConnection("Filename=" + _STR_DB_FILENAME))
            {
                // Open connection
                db.Open();

                SqliteCommand queryCommand = new SqliteCommand();
                queryCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                if (startRow == 0 && endRow == 0)                               // No input parameters provided
                {
                    queryCommand.CommandText = "SELECT [Primary_Key], [PositionID], [ImageData], [PieceID], [PieceName] FROM [" + _STR_TABLE_NAME + "]";
                }                    
                else if (endRow >= 0 && startRow == 0)                          // Select data from start up to endRow
                {
                    queryCommand.CommandText = "SELECT [Primary_Key], [PositionID], [ImageData], [PieceID], [PieceName] FROM [" + _STR_TABLE_NAME + "] LIMIT @_endRow";
                    queryCommand.Parameters.AddWithValue("@_endRow", endRow);
                }
                else if (startRow > 0 && endRow > 0)                            // Select data from start row to end row
                {
                    queryCommand.CommandText = "SELECT [Primary_Key], [PositionID], [ImageData], [PieceID], [PieceName] FROM [" + _STR_TABLE_NAME + "] LIMIT @_startRow, @_endRow";
                    queryCommand.Parameters.AddWithValue("@_startRow", startRow);
                    queryCommand.Parameters.AddWithValue("@_endRow", endRow);
                }
                    
                // Start query command
                using (queryCommand)
                {
                    using (SqliteDataReader reader = queryCommand.ExecuteReader())
                    {
                        // For each row in PiecesTable
                        while (reader.Read())
                        {
                            // Instantiate a new PiecesTableRowInstance Object and populate the columns from the database
                            row = new PiecesTableRowInstance();
                            row.RowID = Convert.ToInt32(reader["Primary_Key"]);
                            row.PositionID = (string)reader["PositionID"];
                            row.PositionImageByte = (byte[])reader["ImageData"];
                            row.PieceID = Convert.ToInt32(reader["PieceID"]);
                            row.PieceName = (string)reader["PieceName"];
                            results.Add(row);
                        }
                        // Close the reader connection
                        reader.Close();
                    }
                }
                // Close the database connection
                db.Close();
            }
            return results;
        }
    }
}

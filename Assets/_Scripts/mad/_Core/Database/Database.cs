using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;

using UnityEngine;

namespace WGRF.Bus
{
    public class Database
    {
        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.companyName);
        ///<summary>A static string which stores the app database absolute path.</summary>
        static string dbPath = string.Empty;
        ///<summary>A static string which contains the app database formatted connection path.</summary>
        static string dbConnectionPath = string.Empty;

        ///<summary>Returns the App Data folder absolute path of the app</summary>
        public string AppDataPath => appDataPath;
        ///<summary>Returns the app database absolute path</summary>
        public string DatabasePath => dbPath;

        public Database()
        {
            //Setup
            HandleAppDataDirectory(appDataPath);

            dbPath = GenerateDatabasePath(appDataPath);

            ConstructDatabase(dbPath);

            dbConnectionPath = "Data Source=" + dbPath;

            //After setup
            OnCreation();
        }

        ///<summary>Call to create the application folder inside the sourcePath path.</summary>
        void HandleAppDataDirectory(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
            { Directory.CreateDirectory(sourcePath); }
        }

        /// <summary>
        /// Genarates the database absolute path and its associated folders
        /// </summary>
        /// <returns>The generated path.</returns>
        string GenerateDatabasePath(string _appDataPath)
        {
            string temp = string.Empty;

            temp = Path.Combine(_appDataPath, "Database", "wgrfDb.db");

            if (!Directory.Exists(Path.Combine(_appDataPath, "Database")))
            { Directory.CreateDirectory(Path.Combine(_appDataPath, "Database")); }

            return temp;
        }

        ///<summary>Constructs database used for various game functions.</summary>
        void ConstructDatabase(string _dbPath)
        {
            if (!File.Exists(_dbPath))
            {
                using (FileStream fs = File.Create(_dbPath)) { }
            }
        }

        ///<summary>Call this method to initiate the database tables creation</summary>
        void OnCreation()
        {
            CreateDatabaseLogger(dbConnectionPath);

            CreateHighscoresTable(dbConnectionPath);
        }

        ///<summary>Creates a table used for logging of the database queries.</summary>
        void CreateDatabaseLogger(string _dbPath)
        {
            using (SqliteConnection connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS DB_LOGGER(
                                                LogFile VARCHAR(255) 
                                            );";


                    command.ExecuteNonQuery();
                }
            }
        }

        ///<summary>Call to add the passed string to the logger table of the database.</summary>
        public void AddLoggerEntry(string log)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"INSERT INTO DB_LOGGER 
                                                (LogFile)
                                            VALUES
                                                (@log)";

                    command.Parameters.Add(new SqliteParameter()
                    {
                        ParameterName = "log",
                        Value = $"{DateTime.Now.ToString()} {log}"
                    });

                    _ = command.ExecuteNonQuery();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        ///<summary>Creates a table to hold the high scores of the game.</summary>
        void CreateHighscoresTable(string _dbPath)
        {
            using (SqliteConnection connection = new SqliteConnection(_dbPath))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"CREATE TABLE IF NOT EXISTS highscores (
                                                _rank  INTEGER       DEFAULT ( -1),
                                                _name  VARCHAR (255) UNIQUE ON CONFLICT FAIL
                                                                     DEFAULT dbName
                                                                     PRIMARY KEY,
                                                _score INTEGER       DEFAULT ( -1) 
                                            );";

                    int result = command.ExecuteNonQuery();

#if UNITY_EDITOR
                    AddLoggerEntry($"High scores table creation result: {result.ToString()}");
#endif
                }
            }
        }

        /// <summary>
        /// Adds a new player record information to the database
        /// </summary>
        /// <param name="record">The externally created player record.</param>
        public bool AddPlayerRecord(PlayerRecord record)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"INSERT INTO highscores 
                                                (_rank, _name, _score)
                                            VALUES
                                                (@_rank, @_name, @_score)";

                    command.Parameters.AddRange(new[]
                    {
                        new SqliteParameter()
                        {
                            ParameterName = "_rank",
                            Value = record.Rank
                        },
                        new SqliteParameter()
                        {
                            ParameterName = "_name",
                            Value = record.Name
                        },
                        new SqliteParameter()
                        {
                            ParameterName = "_score",
                            Value = record.Score
                        }
                    });

                    int result = -1;

                    try
                    {
                        result = command.ExecuteNonQuery();
                    }
                    catch (SqliteException ex)
                    {
                        _ = ex;
#if UNITY_EDITOR
                        AddLoggerEntry($"Failed to add new player record at highscores table. Passed name: {record.Name}");
#endif 
                        result = -1;
                    }

#if UNITY_EDITOR
                    AddLoggerEntry($"New player record insertion with name {record.Name}, result: {result.ToString()}");
#endif
                    return result == 1;
                }
            }
        }

        //@TODO: Update, delete player records
    }
}
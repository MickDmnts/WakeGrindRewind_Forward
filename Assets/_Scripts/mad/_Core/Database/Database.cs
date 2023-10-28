using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
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
            Task.Run(() =>
            {
                //Setup
                HandleAppDataDirectory(appDataPath);

                dbPath = GenerateDatabasePath(appDataPath);

                ConstructDatabase(dbPath);

                dbConnectionPath = "Data Source=" + dbPath;

                //After setup
                OnCreation();
            });
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
            string temp = Path.Combine(_appDataPath, "Database", "wgrfDb.db");

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
        async void CreateDatabaseLogger(string _dbPath)
        {
            using (SqliteConnection connection = new SqliteConnection(_dbPath))
            {
                await connection.OpenAsync();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS DB_LOGGER(
                                                LogFile VARCHAR(255) 
                                            );";


                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
        }

        ///<summary>Call to add the passed string to the logger table of the database.</summary>
        public async void AddLoggerEntry(string log)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                await connection.OpenAsync();

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

                    _ = await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
        }

        ///<summary>Creates a table to hold the high scores of the game.</summary>
        async void CreateHighscoresTable(string _dbPath)
        {
            using (SqliteConnection connection = new SqliteConnection(_dbPath))
            {
                await connection.OpenAsync();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"CREATE TABLE IF NOT EXISTS highscores (
                                                _rank  INTEGER       DEFAULT ( -1) 
                                                                        UNIQUE ON CONFLICT FAIL,
                                                _name  VARCHAR (255) UNIQUE ON CONFLICT FAIL
                                                                        DEFAULT dbName
                                                                        PRIMARY KEY,
                                                _score INTEGER       DEFAULT ( -1) 
                                            );";

                    int result = await command.ExecuteNonQueryAsync();

#if UNITY_EDITOR
                    AddLoggerEntry($"High scores table creation result: {result.ToString()}");
#endif
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();
            }
        }

        /// <summary>
        /// Adds a new player record information to the database
        /// </summary>
        /// <param name="record">The externally created player record.</param>
        public async Task<bool> AddPlayerRecord(PlayerRecord record)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                int result = -1;
                await connection.OpenAsync();

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

                    try
                    {
                        result = await command.ExecuteNonQueryAsync();
                    }
                    catch (SqliteException ex)
                    {
                        _ = ex;
#if UNITY_EDITOR
                        AddLoggerEntry($"Failed to INSERT new player record at highscores table. Passed name: {record.Name}");
#endif 
                        result = -1;
                    }

#if UNITY_EDITOR
                    AddLoggerEntry($"New player record INSERT with name {record.Name}, result: {result.ToString()}");
#endif
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();

                return result == 1;
            }
        }

        #region UPDATE
        /// <summary>
        /// Updates an existing player record information to the database.
        /// </summary>
        /// <param name="recordName">The existing player database entry to update.</param>
        /// <param name="record">The player record containing the new player information.</param>
        public async Task<bool> UpdatePlayerRecord(string recordName, PlayerRecord record)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                int result = -1;
                await connection.OpenAsync();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"UPDATE highscores
                                               SET _rank = @_rank,
                                                   _name = @_name,
                                                   _score = @_score
                                             WHERE _name = @_recordName;";

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
                        },
                        new SqliteParameter()
                        {
                            ParameterName = "_recordName",
                            Value = recordName
                        }
                    });

                    try
                    {
                        result = await command.ExecuteNonQueryAsync();
                    }
                    catch (SqliteException ex)
                    {
                        _ = ex;
#if UNITY_EDITOR
                        AddLoggerEntry($"Failed to UPDATE the player record at highscores table. Passed name: {recordName}");
#endif 
                        result = -1;
                    }

#if UNITY_EDITOR
                    AddLoggerEntry($"Player record UPDATE with name {recordName}, result: {result.ToString()}");
#endif
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();

                return result == 1;
            }
        }

        /// <summary>
        /// Updates an existing player record information to the database.
        /// </summary>
        /// <param name="recordRank">The existing player of X rank to update the information</param>
        /// <param name="record">The player record containing the new player information.</param>
        public async Task<bool> UpdatePlayerRecord(int recordRank, PlayerRecord record)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                int result = -1;
                await connection.OpenAsync();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"UPDATE highscores
                                               SET _rank = @_rank,
                                                   _name = @_name,
                                                   _score = @_score
                                             WHERE _rank = @_recordRank;";

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
                        },
                        new SqliteParameter()
                        {
                            ParameterName = "_recordRank",
                            Value = recordRank
                        }
                    });

                    try
                    {
                        result = await command.ExecuteNonQueryAsync();
                    }
                    catch (SqliteException ex)
                    {
                        _ = ex;
#if UNITY_EDITOR
                        AddLoggerEntry($"Failed to UPDATE the player record at highscores table. Passed rank: {recordRank}");
#endif 
                        result = -1;
                    }

#if UNITY_EDITOR
                    AddLoggerEntry($"Player record UPDATE with rank {recordRank}, result: {result.ToString()}");
#endif
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();

                return result == 1;
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Deletes an existing player record information from the database.
        /// </summary>
        /// <param name="recordName">The player record name to delete from the database.</param>
        public async Task<bool> DeletePlayerRecord(string recordName)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                int result = -1;
                await connection.OpenAsync();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"DELETE FROM highscores
                                                WHERE _name = @_recordName;";

                    command.Parameters.Add(
                        new SqliteParameter()
                        {
                            ParameterName = "_recordName",
                            Value = recordName
                        }
                    );

                    try
                    {
                        result = await command.ExecuteNonQueryAsync();
                    }
                    catch (SqliteException ex)
                    {
                        _ = ex;
#if UNITY_EDITOR
                        AddLoggerEntry($"Failed to DELETE the player record at highscores table. Passed name: {recordName}");
#endif 
                        result = -1;
                    }

#if UNITY_EDITOR
                    AddLoggerEntry($"Player record DELETE with name {recordName}, result: {result.ToString()}");
#endif
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();

                return result == 1;
            }
        }

        /// <summary>
        /// Deletes an existing player record information from the database.
        /// </summary>
        /// <param name="recordName">The player record name to delete from the database.</param>
        public async Task<bool> DeletePlayerRecord(int recordRank)
        {
            using (SqliteConnection connection = new SqliteConnection(dbConnectionPath))
            {
                int result = -1;
                await connection.OpenAsync();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;

                    command.CommandText = @"DELETE FROM highscores
                                                WHERE _rank = @_recordRank;";

                    command.Parameters.Add(
                        new SqliteParameter()
                        {
                            ParameterName = "_recordRank",
                            Value = recordRank
                        }
                    );

                    try
                    {
                        result = await command.ExecuteNonQueryAsync();
                    }
                    catch (SqliteException ex)
                    {
                        _ = ex;
#if UNITY_EDITOR
                        AddLoggerEntry($"Failed to DELETE the player record at highscores table. Passed rank: {recordRank}");
#endif 
                        result = -1;
                    }

#if UNITY_EDITOR
                    AddLoggerEntry($"Player record DELETE with rank {recordRank}, result: {result.ToString()}");
#endif
                }

                await connection.CloseAsync();
                await connection.DisposeAsync();

                return result == 1;
            }
        }
        #endregion
    }
}
﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpTree.DBControl
{
    internal class SongDao
    {
        SqliteController _sqliteController;
        public SongDao(SqliteController sqliteController)
        {
            _sqliteController = sqliteController;
        }
        public void InitializeTable()
        {
            string query = @"
            CREATE TABLE IF NOT EXISTS Songs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Path TEXT NOT NULL,
                Size INTEGER NOT NULL,
                Duration INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Author TEXT,
                Album TEXT,
                Year TEXT,
                Genres TEXT
            );
            ";

            _sqliteController.Connect();
            _sqliteController.ExecuteUpdate(query);  // Execute a query to create the table if it doesn't exist
            _sqliteController.Disconnect();
        }
        public void DropTable()
        {
            string query = "DROP TABLE IF EXISTS Songs;";

            _sqliteController.Connect();
            _sqliteController.ExecuteUpdate(query);  // Execute the query to drop the table if it exists
            _sqliteController.Disconnect();
        }
        public List<SongModel> GetAllSongs()
        {
            var songs = new List<SongModel>();
            string query = "SELECT Path, Size, Duration, Name, Author, Album, Year, Genres FROM Songs";

            _sqliteController.Connect();
            using (var reader = _sqliteController.ExecuteQuery(query))
            {
                while (reader.Read())
                {
                    songs.Add(new SongModel(
                        reader["Path"].ToString(),
                        Convert.ToInt64(reader["Size"]),
                        Convert.ToInt64(reader["Duration"]),
                        reader["Name"].ToString(),
                        reader["Author"].ToString(),
                        reader["Album"].ToString(),
                        reader["Year"].ToString(),
                        reader["Genres"].ToString()
                    ));
                }
            }
            _sqliteController.Disconnect();

            return songs;
        }

        public SongModel GetSong(string name)
        {
            string query = "SELECT Path, Size, Duration, Name, Author, Album, Year, Genres FROM Songs WHERE Name = @Name";
            var parameters = new SQLiteParameter[]
            {
            new SQLiteParameter("@Name", name)
            };

            _sqliteController.Connect();
            using (var reader = _sqliteController.ExecuteQueryWithParams(query, parameters))
            {
                if (reader.Read())
                {
                    var song = new SongModel(
                        reader["Path"].ToString(),
                        Convert.ToInt64(reader["Size"]),
                        Convert.ToInt64(reader["Duration"]),
                        reader["Name"].ToString(),
                        reader["Author"].ToString(),
                        reader["Album"].ToString(),
                        reader["Year"].ToString(),
                        reader["Genres"].ToString()
                    );
                    _sqliteController.Disconnect();
                    return song;
                }
            }
            _sqliteController.Disconnect();

            return null;
        }

        public int CreateSong(SongModel song)
        {
            string query = "INSERT INTO Songs (Path, Size, Duration, Name, Author, Album, Year, Genres) " +
                           "VALUES (@Path, @Size, @Duration, @Name, @Author, @Album, @Year, @Genres)";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@Path", song.Path),
                new SQLiteParameter("@Size", song.Size),
                new SQLiteParameter("@Duration", song.Duration),
                new SQLiteParameter("@Name", song.Name),
                new SQLiteParameter("@Author", song.Author),
                new SQLiteParameter("@Album", song.Album),
                new SQLiteParameter("@Year", song.Year),
                new SQLiteParameter("@Genres", song.Genres)
            };

            _sqliteController.Connect();
            int result = _sqliteController.ExecuteUpdateWithParams(query, parameters);
            _sqliteController.Disconnect();

            return result;
        }

        public int UpdateSong(SongModel song)
        {
            string query = "UPDATE Songs SET Path = @Path, Size = @Size, Duration = @Duration, " +
                           "Author = @Author, Album = @Album, Year = @Year, Genres = @Genres WHERE Name = @Name";
            var parameters = new SQLiteParameter[]
            {
            new SQLiteParameter("@Path", song.Path),
            new SQLiteParameter("@Size", song.Size),
            new SQLiteParameter("@Duration", song.Duration),
            new SQLiteParameter("@Name", song.Name),
            new SQLiteParameter("@Author", song.Author),
            new SQLiteParameter("@Album", song.Album),
            new SQLiteParameter("@Year", song.Year),
            new SQLiteParameter("@Genres", song.Genres)
            };

            _sqliteController.Connect();
            int result = _sqliteController.ExecuteUpdateWithParams(query, parameters);
            _sqliteController.Disconnect();

            return result;
        }

        public int DeleteSong(string path)
        {
            string query = "DELETE FROM Songs WHERE Path = @Path";
            var parameters = new SQLiteParameter[]
            {
            new SQLiteParameter("@Path", path)
            };

            _sqliteController.Connect();
            int result = _sqliteController.ExecuteUpdateWithParams(query, parameters);
            _sqliteController.Disconnect();

            return result;
        }
    }
}
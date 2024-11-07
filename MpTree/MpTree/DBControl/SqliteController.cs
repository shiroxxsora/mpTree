using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MpTree.DBControl
{
    internal class SqliteController
    {
        private SQLiteConnection connection;
        private string connectionString;
        public SqliteController(string connectionString)
        {
            this.connectionString = connectionString;
            connection = new SQLiteConnection(connectionString);
        }
                

        public void Connect()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void Disconnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        public SQLiteDataReader ExecuteQuery(string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                return command.ExecuteReader();
            }
        }
        public SQLiteDataReader ExecuteQueryWithParams(string query, SQLiteParameter[] parameters)
        {
            using(var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteReader();
            }
        }

        public int ExecuteUpdate(string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteUpdateWithParams(string query, SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }
    }
}

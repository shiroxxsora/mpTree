using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MpTree.DBControl
{
    /// <summary>
    /// Контроллер для работы с базой данных SQLite.
    /// </summary>
    public class SqliteController
    {
        private readonly SQLiteConnection connection;
        private readonly string connectionString;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SqliteController"/>.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public SqliteController(string connectionString)
        {
            this.connectionString = connectionString;
            connection = new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// Устанавливает соединение с базой данных.
        /// </summary>
        public void Connect()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Закрывает соединение с базой данных.
        /// </summary>
        public void Disconnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Выполняет SQL-запрос на чтение данных.
        /// </summary>
        /// <param name="query">SQL-запрос для выполнения.</param>
        /// <returns>Объект для чтения результатов запроса.</returns>
        public SQLiteDataReader ExecuteQuery(string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Выполняет параметризованный SQL-запрос на чтение данных.
        /// </summary>
        /// <param name="query">SQL-запрос для выполнения.</param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <returns>Объект для чтения результатов запроса.</returns>
        public SQLiteDataReader ExecuteQueryWithParams(string query, SQLiteParameter[] parameters)
        {
            using(var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Выполняет SQL-запрос на изменение данных.
        /// </summary>
        /// <param name="query">SQL-запрос для выполнения.</param>
        /// <returns>Количество затронутых строк.</returns>
        public int ExecuteUpdate(string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Выполняет параметризованный SQL-запрос на изменение данных.
        /// </summary>
        /// <param name="query">SQL-запрос для выполнения.</param>
        /// <param name="parameters">Параметры запроса.</param>
        /// <returns>Количество затронутых строк.</returns>
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

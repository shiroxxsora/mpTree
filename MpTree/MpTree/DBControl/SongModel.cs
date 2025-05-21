using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;

namespace MpTree.DBControl
{
    /// <summary>
    /// Представляет музыкальный файл с его метаданными.
    /// </summary>
    public class SongModel
    {
        private string _path;
        private long _size;
        private long _duration;
        private string _name;
        private string _author;
        private string _album;
        private string _year;
        private string _genres;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SongModel"/>.
        /// </summary>
        /// <param name="path">Путь к файлу песни. Должен быть корректным путем Windows.</param>
        /// <param name="size">Размер файла песни в байтах.</param>
        /// <param name="duration">Длительность песни в секундах. Должна быть больше 0.</param>
        /// <param name="name">Название или заголовок песни.</param>
        /// <param name="author">Автор или исполнитель песни.</param>
        /// <param name="album">Альбом, к которому принадлежит песня.</param>
        /// <param name="year">Год выпуска песни.</param>
        /// <param name="genres">Жанры песни.</param>
        /// <exception cref="ArgumentException">Выбрасывается, если путь недействителен или длительность не положительна.</exception>
        public SongModel(string path, long size, long duration, string name, string author, string album, string year, string genres)
        {
            string windowsPathPattern = @"^[a-zA-Z]:\\(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*$";
            if (string.IsNullOrWhiteSpace(path) || !Regex.IsMatch(path, windowsPathPattern))
            {
                throw new ArgumentException($"Неверный путь: {path}", nameof(path));
            }
            if (duration <= 0)
            {
                throw new ArgumentException($"Длительность должна быть больше 0. Значение: {duration}", nameof(duration));
            }

            _path = path;
            _size = size; 
            _duration = duration; 
            _name = name;
            _author = author;
            _album = album;
            _year = year;
            _genres = genres;
        }

        /// <summary>
        /// Получает XML-представление модели песни.
        /// </summary>
        /// <remarks>
        /// Это свойство может быть более подходящим как метод, если оно включает значительную обработку
        /// или если оно в основном используется для конкретных сценариев экспорта, а не для общего доступа к данным.
        /// Для ответов API сериализация JSON обычно обрабатывается ASP.NET Core напрямую из свойств модели.
        /// </remarks>
        public string GetXmlString 
        {
            get
            {
                void AppendElement(XmlDocument xmlDoc, XmlElement parent, string elementName, string elementValue)
                {
                    XmlElement element = xmlDoc.CreateElement(elementName);
                    element.InnerText = elementValue ?? string.Empty;
                    parent.AppendChild(element);
                }
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement root = xmlDoc.CreateElement("Model");

                AppendElement(xmlDoc, root, "Path", Path);
                AppendElement(xmlDoc, root, "Size", Size.ToString());
                AppendElement(xmlDoc, root, "Duration", Duration.ToString());
                AppendElement(xmlDoc, root, "Name", Name);
                AppendElement(xmlDoc, root, "Author", Author);
                AppendElement(xmlDoc, root, "Album", Album);
                AppendElement(xmlDoc, root, "Year", Year);
                AppendElement(xmlDoc, root, "Genres", Genres);

                xmlDoc.AppendChild(root);
                return xmlDoc.OuterXml;
            }
        }
        
        /// <summary>
        /// Получает путь к файлу песни.
        /// </summary>
        public string Path { get { return _path; } }

        /// <summary>
        /// Получает размер файла песни в байтах.
        /// </summary>
        public long Size { get { return _size; } }

        /// <summary>
        /// Получает длительность песни в секундах.
        /// </summary>
        public long Duration { get { return _duration; } }

        /// <summary>
        /// Получает название или заголовок песни.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Получает автора или исполнителя песни.
        /// </summary>
        public string Author { get { return _author; } }

        /// <summary>
        /// Получает альбом, к которому принадлежит песня.
        /// </summary>
        public string Album { get { return _album; } }

        /// <summary>
        /// Получает год выпуска песни.
        /// </summary>
        public string Year { get { return _year; } }

        /// <summary>
        /// Получает жанры песни.
        /// </summary>
        public string Genres { get { return _genres; } }
    }
}

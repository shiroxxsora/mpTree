using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
namespace MpTree.DBControl
{
    internal class SongModel
    {
        private string _path;
        private long _size;
        private long _duration;
        private string _name;
        private string _author;
        private string _album;
        private string _year;
        private string _genres;
        public SongModel(string path, long size, long duration, string name, string author, string album, string year, string genres)
        {
            _path = path;
            _size = size; // Размер файла в байтах
            _duration = duration; // Длительность песни в секундах
            _name = name;
            _author = author;
            _album = album;
            _year = year;
            _genres = genres;
            CheckFields();

        }
        private void CheckFields()
        {
            string windowsPathPattern = @"^[a-zA-Z]:\\(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*$";
            if(!Regex.IsMatch(_path, windowsPathPattern))
            {
                throw new Exception($"Path invalid :{Path}");
            }
            if (Duration <= 0) 
            {
                throw new Exception($"Duration less 0 iMPossible: {Duration}");
            }

        }
        public string GetXmlString
        {
            get
            {
                void AppendElement(XmlDocument xmlDoc, XmlElement parent, string elementName, string elementValue)
                {
                    XmlElement element = xmlDoc.CreateElement(elementName);
                    element.InnerText = elementValue;
                    parent.AppendChild(element);
                }
                // Create an XmlDocument to build the XML
                XmlDocument xmlDoc = new XmlDocument();

                // Create the root element
                XmlElement root = xmlDoc.CreateElement("Model");

                // Create and append each element with respective property values
                AppendElement(xmlDoc, root, "Path", Path);
                AppendElement(xmlDoc, root, "Size", Size.ToString());
                AppendElement(xmlDoc, root, "Duration", Duration.ToString());
                AppendElement(xmlDoc, root, "Name", Name);
                AppendElement(xmlDoc, root, "Author", Author);
                AppendElement(xmlDoc, root, "Album", Album);
                AppendElement(xmlDoc, root, "Year", Year);
                AppendElement(xmlDoc, root, "Genres", Genres);

                // Append the root element to the XmlDocument
                xmlDoc.AppendChild(root);

                // Return the XML as a string
                return xmlDoc.OuterXml;
            }
        }

        // Helper method to create an element and append it

        public string Path { get { return _path; } }
        public long Size { get { return _size; } }
        public long Duration { get { return _duration; } }
        public string Name { get { return _name; } }
        public string Author { get { return _author; } }
        public string Album { get { return _album; } }
        public string Year { get { return _year; } }
        public string Genres { get { return _genres; } }
    }
}

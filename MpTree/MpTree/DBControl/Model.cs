using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace MpTree.DBControl
{
    internal class Model
    {
        private string _path;
        private string _size;
        private string _duration;
        private string _name;
        private string _author;
        private string _albom;
        private string _year;
        private string _genres;
        public Model(string path, string size, string duration, string name, string author, string albom, string year, string genres)
        {
            _path = path;
            _size = size;
            _duration = duration;
            _name = name;
            _author = author;
            _albom = albom;
            _year = year;
            _genres = genres;
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
                AppendElement(xmlDoc, root, "Path", _path);
                AppendElement(xmlDoc, root, "Size", _size);
                AppendElement(xmlDoc, root, "Duration", _duration);
                AppendElement(xmlDoc, root, "Name", _name);
                AppendElement(xmlDoc, root, "Author", _author);
                AppendElement(xmlDoc, root, "Albom", _albom);
                AppendElement(xmlDoc, root, "Year", _year);
                AppendElement(xmlDoc, root, "Genres", _genres);

                // Append the root element to the XmlDocument
                xmlDoc.AppendChild(root);

                // Return the XML as a string
                return xmlDoc.OuterXml;
            }
        }

        // Helper method to create an element and append it

        public string Path { get { return _path; } }
        public string Size { get { return _size; } }
        public string Duration { get { return _duration; } }
        public string Name { get { return _name; } }
        public string Author { get { return _author; } }
        public string Albom { get { return _albom; } }
        public string Year { get { return _year; } }
        public string Genres { get { return _genres; } }
    }
}

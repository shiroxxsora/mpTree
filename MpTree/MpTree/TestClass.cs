using MpTree.DBControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Networking.Connectivity;

namespace MpTree
{
    internal class TestClass
    {
        private string _connectionString;
        private SqliteController _controller;
        private SongDao _songDao;
        public TestClass()
        {
            string _connectionString = "Data Source=mydatabase.db;Version=3;";
            _controller = new SqliteController(_connectionString);
            _songDao = new SongDao(_controller);
            _songDao.DropTable();
            _songDao.InitializeTable();
        }
        public void Start()
        {
            CreateTest();
        }
        public bool CreateTest()
        {
            var model = new SongModel("C:\\path\\to\\file", 150 * 1024 * 1024, 3 * 60 + 45, "Song Name", "Artist", "Album Name", "2024", "Pop");
            var xml = model.GetXmlString;

            var result = _songDao.CreateSong(model);
            var xmlresult = _songDao.GetSong(model.Name).GetXmlString;

            if (result != 1)
            {
                throw new Exception("Query Error");
            }
            if (!AreXmlStringsEquivalent(xml, xmlresult))
            {
                throw new Exception($"{xml} != {xmlresult}");
            }
            return true;
        }
        static bool AreXmlStringsEquivalent(string xml1, string xml2)
        {
            try
            {
                XmlDocument doc1 = new XmlDocument();
                doc1.LoadXml(xml1);
                XmlDocument doc2 = new XmlDocument();
                doc2.LoadXml(xml2);

                doc1.Normalize();
                doc2.Normalize();

                return doc1.OuterXml == doc2.OuterXml;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during XML comparison: {ex.Message}");
                return false;
            }
        }
    }
}

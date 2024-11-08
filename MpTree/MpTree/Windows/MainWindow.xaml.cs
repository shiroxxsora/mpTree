using Microsoft.Win32;
using MpTree.DBControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MpTree.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string _connectionString = "Data Source=mydatabase.db;Version=3;";
        static SqliteController _controller = new SqliteController(_connectionString);
        static SongDao _songDao = new SongDao(_controller);
        public MainWindow()
        {
            InitializeComponent();
            _songDao.InitializeTable();
            LoadXmlData();
        }
        private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedFiles = openFileDialog.FileNames;
                foreach (string filePath in selectedFiles) {
                    var file = TagLib.File.Create(filePath);
                    FileInfo fileInfo = new FileInfo(filePath);

                    _songDao.CreateSong(new SongModel(
                        filePath,
                        fileInfo.Length,
                        (long)file.Properties.Duration.TotalSeconds,
                        file.Tag.Title,
                        file.Tag.FirstPerformer,
                        file.Tag.Album,
                        file.Tag.Year.ToString(),
                        string.Join(", ", file.Tag.Genres)
                        ));
                }

                LoadXmlData();
            }
        }

        private void ShowDuplicatesButton_Click( object sender, RoutedEventArgs e)
        {
            LoadXmlDublicateData();
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            LoadXmlData();
        }
        private void ClearDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            _songDao.DropTable();
            _songDao.InitializeTable();
            LoadXmlData();
        }

        private void LoadXmlDublicateData()
        {
            List<SongModel> SongList = _songDao.GetAllSongs();

            var allData = new List<dynamic>(); // Список для хранения данных

            foreach (var song in SongList)
            {
                string xmlString = song.GetXmlString;

                try
                {
                    XDocument xDoc = XDocument.Parse(xmlString);

                    // Пример извлечения данных. Здесь мы извлекаем все элементы <Model> в каждом XML
                    var items = xDoc.Descendants("Model")
                                    .Select(item => new
                                    {
                                        Path = item.Element("Path")?.Value,
                                        Size = item.Element("Size")?.Value,
                                        Duration = item.Element("Duration")?.Value,
                                        Name = item.Element("Name")?.Value,
                                        Author = item.Element("Author")?.Value,
                                        Albom = item.Element("Albom")?.Value,
                                        Year = item.Element("Year")?.Value,
                                        Genres = item.Element("Genres")?.Value
                                    })
                                    .ToList();

                    allData.AddRange(items);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Ошибка при обработке XML: {e.Message}");
                }
            }

            // Группировка по полю Name и выбор только дубликатов
            var duplicates = allData
                .GroupBy(item => new { item.Size, item.Duration, item.Name, item.Albom, item.Year, item.Genres  }) // Группировка по всем полям кроме path
                .Where(group => group.Count() > 1) // Выбор только тех групп, где больше одного элемента
                .SelectMany(group => group) // Преобразуем группы обратно в список элементов
                .ToList();

            // Отображаем только дубликаты в DataGrid
            XmlDataGrid.ItemsSource = duplicates;
        }

        private void LoadXmlData()
        {
            List<SongModel> SongList = _songDao.GetAllSongs();

            var allData = new List<dynamic>(); // Список для хранения данных

            foreach (var song in SongList)
            {
                string xmlString = song.GetXmlString;

                try
                {
                    XDocument xDoc = XDocument.Parse(xmlString);

                    // Пример извлечения данных. Здесь мы извлекаем все элементы <Model> в каждом XML
                    var items = xDoc.Descendants("Model")
                                    .Select(item => new
                                    {
                                        Path = item.Element("Path")?.Value,
                                        Size = item.Element("Size")?.Value,
                                        Duration = item.Element("Duration")?.Value,
                                        Name = item.Element("Name")?.Value,
                                        Author = item.Element("Author")?.Value,
                                        Albom = item.Element("Albom")?.Value,
                                        Year = item.Element("Year")?.Value,
                                        Genres = item.Element("Genres")?.Value
                                    })
                                    .ToList();

                    allData.AddRange(items);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке XML: {ex.Message}");
                }
            }

            // Привязка данных к DataGrid
            XmlDataGrid.ItemsSource = allData;
        }
    }
}

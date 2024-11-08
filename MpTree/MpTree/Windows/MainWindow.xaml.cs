using Microsoft.Win32;
using MpTree.DBControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using MessageBox = System.Windows.MessageBox;

namespace MpTree.Windows
{
    public partial class MainWindow : Window
    {
        private static string xmlFilePath = "Mp3Data.xml";

        public MainWindow()
        {
            InitializeComponent();
            LoadXmlData();
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFolder = folderDialog.SelectedPath;
                await SaveMp3DataToXmlAsync(selectedFolder);
                LoadXmlData();
            }
        }

        private void ShowAllButton_Click(object sender, RoutedEventArgs e) 
        { 
            LoadXmlData();
        }

        private async Task SaveMp3DataToXmlAsync(string folderPath)
        {
            try
            {
                // Получаем все MP3 файлы в папке и поддиректориях
                var mp3Files = Directory.EnumerateFiles(folderPath, "*.mp3", SearchOption.AllDirectories)
                                        .Where(file => HasReadAccess(file)); // Проверка на доступ

                var mp3Data = new List<XElement>();

                foreach (var filePath in mp3Files)
                {
                    try
                    {
                        var file = await Task.Run(() => TagLib.File.Create(filePath));
                        var fileInfo = new FileInfo(filePath);

                        var fileElement = new XElement("File",
                            new XElement("Path", filePath),
                            new XElement("Size", fileInfo.Length),
                            new XElement("Duration", (long)file.Properties.Duration.TotalSeconds),
                            new XElement("Name", file.Tag.Title ?? ""),
                            new XElement("Author", file.Tag.FirstPerformer ?? ""),
                            new XElement("Albom", file.Tag.Album ?? ""),
                            new XElement("Year", file.Tag.Year.ToString()),
                            new XElement("Genres", string.Join(", ", file.Tag.Genres))
                        );

                        mp3Data.Add(fileElement);
                    }
                    catch (Exception ex)
                    {
                        // Если возникла ошибка при обработке конкретного файла, выводим сообщение, но продолжаем обработку
                        MessageBox.Show($"Ошибка при обработке файла {filePath}: {ex.Message}");
                    }
                }

                // Создаем и сохраняем XML-документ
                var xDocument = new XDocument(new XElement("Mp3Files", mp3Data));
                xDocument.Save(xmlFilePath);

                MessageBox.Show("Данные MP3 файлов сохранены в XML.");
            }
            catch (UnauthorizedAccessException ex)
            {
                // Сообщение при отсутствии доступа к папке
                MessageBox.Show($"Нет доступа к папке: {folderPath}. Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файлов: {ex.Message}");
            }
        }
        private bool HasReadAccess(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open, FileAccess.Read)) { }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void LoadXmlData()
        {
            // Проверяем, существует ли XML-файл
            if (!File.Exists(xmlFilePath))
            {
                MessageBox.Show("XML файл не найден.");
                return;
            }

            // Загружаем XML-документ
            var xDocument = XDocument.Load(xmlFilePath);
            var allData = xDocument.Descendants("File")
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

            // Привязываем данные к DataGrid
            XmlDataGrid.ItemsSource = allData;
        }

        private void ShowDuplicatesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(xmlFilePath))
            {
                MessageBox.Show("XML файл не найден.");
                return;
            }

            var xDocument = XDocument.Load(xmlFilePath);
            var allData = xDocument.Descendants("File")
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

            // Группировка и фильтрация дубликатов (по всем полям, кроме Path)
            var duplicates = allData
                .GroupBy(item => new { item.Size, item.Duration, item.Name, item.Albom, item.Year, item.Genres })
                .Where(group => group.Count() > 1)
                .SelectMany(group => group)
                .ToList();

            XmlDataGrid.ItemsSource = duplicates;
        }

        private void ClearDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(xmlFilePath))
            {
                File.Delete(xmlFilePath);
            }

            XmlDataGrid.ItemsSource = null;
            MessageBox.Show("XML файл и данные были удалены.");
        }
    }
}

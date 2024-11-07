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
        public MainWindow(string path)
        {
            InitializeComponent();
            LoadXmlToDataGrid(path); // Укажите путь к XML-файлу
        }
        private void LoadXmlToDataGrid(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Загрузка XML-файла
                    XDocument xdoc = XDocument.Load(filePath);

                    // Преобразуем XML-данные в список анонимных объектов для привязки к DataGrid
                    var data = xdoc.Root.Elements("Employee") // Используйте ваш корневой элемент
                        .Select(e => new
                        {
                            ID = (int)e.Element("ID"),
                            Name = (string)e.Element("Name"),
                            Position = (string)e.Element("Position"),
                            Salary = (decimal)e.Element("Salary")
                        }).ToList();

                    // Привязка данных к DataGrid
                    XmlDataGrid.ItemsSource = data;
                }
                else
                {
                    MessageBox.Show("Файл не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
            }
        }
    }
}

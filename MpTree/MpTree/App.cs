using MpTree;
using MpTree.DBControl;
using MpTree.Windows;
using System.IO;
using System.Windows;
using System.Xml.Linq;

class App : Application
{
    [STAThread]
    static void Main(string[] args)
    {
        var app = new App();

        TestClass testClass = new TestClass();
        testClass.Start();
        testClass.Start();


        /*
         Это относительный путь до test.xml, чтобы не указывать полное имя, скажи потом, у тебя работает или нет
         */
        string file = Path.Combine(
           Directory.GetParent(
               Directory.GetParent(
                   Directory.GetParent(
                        Directory.GetParent(Directory.GetCurrentDirectory()).FullName
                   ).FullName
               ).FullName
           ).FullName,
           "test.xml"
       );
        //string testxmlPath = "C:\\Users\\user\\source\\repos\\mpTree\\MpTree\\test.xml";
        string testxmlPath = file;

        var mainWindow = new MainWindow(testxmlPath);
        app.Run(mainWindow);

        if (args.Length == 0)
        {
            Console.WriteLine("Укажите путь к XML файлу в качестве аргумента.");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не найден: " + filePath);
            return;
        }
        try
        {

            // Загружаем XML-файл
            XDocument xmlDoc = XDocument.Load(filePath);

            // Пример: получение корневого элемента и вывод его содержимого
            XElement root = xmlDoc.Root;
            Console.WriteLine("Корневой элемент: " + root.Name);

            // Пример: обход всех дочерних элементов корневого узла
            foreach (XElement element in root.Elements())
            {
                Console.WriteLine("Элемент: " + element.Name + ", значение: " + element.Value);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при разборе XML файла: " + e.Message);
        }
    }
}

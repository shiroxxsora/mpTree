﻿using MpTree;
using MpTree.DBControl;
using MpTree.Windows;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using Application = System.Windows.Application;

class App : Application
{
    [STAThread]
    static void Main(string[] args)
    {
        var app = new App();
        var mainWindow = new MainWindow();
        app.Run(mainWindow);
    }
}

using MpTree;
using MpTree.DBControl;

var model = new Model("C:\\path\\to\\file", "150MB", "3:45", "Song Name", "Artist", "Album Name", "2024", "Pop");
Console.WriteLine(model.GetXmlString);

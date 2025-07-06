using Engine.Core;

var project = new Project();
Console.OutputEncoding = System.Text.Encoding.UTF8;
project.Configure("resources/config/project.yaml");
project.Init();
project.Run();
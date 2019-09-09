using System;
using ElectronCgi.DotNet;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new ConnectionBuilder()
                .WithLogging()
                .Build();
            
            connection.On<string, string>("getPathToImage", name => @"C:\Users\kryst\Downloads\imagesFromPython\img.jpg");
            
            connection.Listen();    
        }
    }
}
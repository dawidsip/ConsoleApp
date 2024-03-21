using System;
using System.Text;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var processor = new DataProcessor();
            processor.ImportData("data.csv");
            processor.PrintData();
        }
    }
}
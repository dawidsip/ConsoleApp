using System;
using System.Text;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new DataProcessor();
            reader.ImportAndPrintData("dataa.csv");
        }
    }
}
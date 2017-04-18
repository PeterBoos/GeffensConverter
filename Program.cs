using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeffensConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            ConverterScript.Run(@"C:\Users\peter.boos\Documents\Mälarenergi\20170201115213_057177.txt", @"C:\Users\peter.boos\Documents\Mälarenergi\Done");

            Console.WriteLine("Done...");
            Console.Read();
        }
    }
}

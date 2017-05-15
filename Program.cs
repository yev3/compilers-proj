using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class Program
    {


        private static List<string> test_files = new List<string>()
        {
            @"TestFiles\good1p.txt",
            @"TestFiles\iftest.txt",
            @"TestFiles\logictest.txt"
        };


        static void Main(string[] args)
        {

            var testNbr = 1;
            foreach (var file in test_files)
            {
                var parser = new TCCLParser();

                Console.WriteLine("Test case {0}:", testNbr++);
                Console.WriteLine("========================================\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(File.ReadAllText(file));
                Console.ResetColor();
                Console.WriteLine();
                parser.Parse(file);
            }
       

            Console.ReadKey();
        }
    }
}

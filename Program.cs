using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//        private static string test_input = @"
//public class good1p {

//   public static void outInt(int n) {
//     java.io.PrintStream ps;
//     ps = java.lang.System.out;
//     ps.print(n);
//   }

//   public static void main431() {
//     int w,x;
//     x = 3+4;  TestClasses.good1p.outInt(x);
//     x = 5*7;  TestClasses.good1p.outInt(x);
//   }
//}
//";

namespace ASTBuilder
{
    class Program
    {
        private static List<string> test_cases = new List<string>()
        {
@"public class good1p {
   public static void outInt(int n) {
     java.io.PrintStream ps;
     ps = java.lang.System.out;
     ps.print(n);
   }
}",
@"public class good2p {
   public static void main431() {
     int w,x;
     x = 3+4;  TestClasses.good1p.outInt(x);
     x = 5*7;  TestClasses.good1p.outInt(x);
   }
}",
        };
        static void Main(string[] args)
        {
            string dir = Environment.CurrentDirectory;
            StreamReader testFile1 = File.OpenText(dir+@"\TestFiles\good1p.txt");
            StreamReader testFile2 = File.OpenText(dir+@"\TestFiles\iftest.txt");
            StreamReader testFile3 = File.OpenText(dir+@"\TestFiles\logictest.txt");

            StreamReader[] fileArray = {testFile1, testFile2, testFile3};
            

            var testNbr = 1;
            foreach (var file in fileArray)
            {
                var parser = new TCCLParser();
                string s = file.ReadToEnd();

                Console.WriteLine("Test case {0}:", testNbr);
                Console.WriteLine("========================================\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(s);
                Console.ResetColor();
                Console.WriteLine();
                Stream strm = new MemoryStream(Encoding.ASCII.GetBytes(s));
                parser.Parse(strm);
                ++testNbr;
            }
       
            //foreach (string test_input in test_cases)
            //{
            //    Console.WriteLine("Test case:");
            //    Console.WriteLine("========================================\n");
            //    Console.ForegroundColor = ConsoleColor.Cyan;
            //    Console.WriteLine(test_input);
            //    Console.ResetColor();
            //    Console.WriteLine();
            //    Console.WriteLine();

            //    Stream strm = new MemoryStream(Encoding.ASCII.GetBytes(test_input));
            //    parser.Parse(strm);
            //}


            Console.ReadKey();
        }
    }
}
